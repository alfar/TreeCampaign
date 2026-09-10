import { useCallback, useEffect, useRef, useState } from "react";
import {
  collectStop,
  correctStop,
  deliverLoad,
  getStopsForTeam,
  getTeam,
  markStopUnresolved,
  reportTrailerFull,
  retryStop,
} from "../api/client";
// HttpError (thrown by client.ts on non-2xx responses) is a real server reply and never
// treated as a connectivity failure — only a fetch-level TypeError means "offline."
import type { Stop } from "../api/models/stop";
import {
  loadTeamStopsState,
  saveTeamStopsState,
  type QueuedAction,
  type QueuedStopActionType,
  type QueuedTeamActionType,
} from "./stopQueueStorage";
import {
  applyOptimisticDelivery,
  applyOptimisticTransition,
  isActionAllowed,
} from "./stopTransitions";

const POLL_INTERVAL_MS = 30_000;

type ActionResult =
  | { kind: "stop"; stop: Stop }
  | { kind: "trailerFull"; isTrailerFull: boolean }
  | { kind: "delivered" };

async function runAction(
  campaignId: string,
  action: QueuedAction,
): Promise<ActionResult> {
  if (action.scope === "team") {
    switch (action.type) {
      case "reportTrailerFull": {
        const team = await reportTrailerFull(campaignId, action.teamId);
        return { kind: "trailerFull", isTrailerFull: team.isTrailerFull ?? false };
      }
      case "deliverLoad":
        await deliverLoad(campaignId, action.teamId);
        return { kind: "delivered" };
    }
  }
  switch (action.type) {
    case "collect":
      return { kind: "stop", stop: await collectStop(campaignId, action.stopId) };
    case "unresolved":
      return { kind: "stop", stop: await markStopUnresolved(campaignId, action.stopId, action.reason) };
    case "retry":
      return { kind: "stop", stop: await retryStop(campaignId, action.stopId) };
    case "correct":
      return { kind: "stop", stop: await correctStop(campaignId, action.stopId) };
  }
}

function isNetworkError(err: unknown): boolean {
  // fetch() rejects with TypeError only for network failures (offline, DNS, timeout via AbortController),
  // never for HTTP error statuses — those resolve normally and are handled by the caller.
  return err instanceof TypeError;
}

export function useTeamStops(campaignId: string, teamId: string) {
  const initial = loadTeamStopsState(campaignId, teamId);
  const [stops, setStops] = useState<Stop[]>(initial.lastKnownStops);
  const [isTrailerFull, setIsTrailerFull] = useState<boolean | null>(
    initial.isTrailerFull,
  );
  const [queue, setQueue] = useState<QueuedAction[]>(initial.queue);
  const [isOffline, setIsOffline] = useState(false);
  const draining = useRef(false);

  const stopsRef = useRef(stops);
  stopsRef.current = stops;
  const trailerFullRef = useRef(isTrailerFull);
  trailerFullRef.current = isTrailerFull;
  const queueRef = useRef(queue);
  queueRef.current = queue;

  const persist = useCallback(
    (nextStops: Stop[], nextTrailerFull: boolean | null, nextQueue: QueuedAction[]) => {
      saveTeamStopsState(campaignId, teamId, {
        lastKnownStops: nextStops,
        isTrailerFull: nextTrailerFull,
        queue: nextQueue,
      });
    },
    [campaignId, teamId],
  );

  const hasPendingDeliverLoad = useCallback(
    () =>
      queueRef.current.some(
        (a) => a.scope === "team" && a.type === "deliverLoad",
      ),
    [],
  );

  const pendingStopIds = useCallback(() => {
    const ids = new Set(
      queueRef.current
        .filter((a): a is Extract<QueuedAction, { scope: "stop" }> => a.scope === "stop")
        .map((a) => a.stopId),
    );
    // A queued deliverLoad optimistically marks every locally-Collected stop as Delivered;
    // keep that local state until the request actually confirms, so a poll landing in
    // between doesn't revert them back to Collected.
    if (hasPendingDeliverLoad()) {
      for (const s of stopsRef.current) {
        if (s.stopType === "Delivered") ids.add(s.id);
      }
    }
    return ids;
  }, [hasPendingDeliverLoad]);

  const hasPendingTeamAction = useCallback(
    () => queueRef.current.some((a) => a.scope === "team"),
    [],
  );

  const poll = useCallback(async () => {
    try {
      const [serverStops, serverTeam] = await Promise.all([
        getStopsForTeam(campaignId, teamId),
        getTeam(campaignId, teamId),
      ]);
      setIsOffline(false);

      const stillPending = pendingStopIds();
      const mergedStops = serverStops.map((serverStop) =>
        stillPending.has(serverStop.id)
          ? (stopsRef.current.find((s) => s.id === serverStop.id) ?? serverStop)
          : serverStop,
      );
      const mergedTrailerFull = hasPendingTeamAction()
        ? trailerFullRef.current
        : serverTeam.isTrailerFull;

      setStops(mergedStops);
      setIsTrailerFull(mergedTrailerFull);
      persist(mergedStops, mergedTrailerFull, queueRef.current);
    } catch (err) {
      if (isNetworkError(err)) {
        setIsOffline(true);
      }
    }
  }, [campaignId, teamId, persist, pendingStopIds, hasPendingTeamAction]);

  const drainQueue = useCallback(async () => {
    if (draining.current) return;
    draining.current = true;
    try {
      while (queueRef.current.length > 0) {
        const [next, ...rest] = queueRef.current;
        try {
          const result = await runAction(campaignId, next);
          setIsOffline(false);
          queueRef.current = rest;
          setQueue(rest);

          if (result.kind === "stop") {
            const merged = stopsRef.current.map((s) =>
              s.id === result.stop.id ? result.stop : s,
            );
            stopsRef.current = merged;
            setStops(merged);
            persist(merged, trailerFullRef.current, rest);
          } else if (result.kind === "trailerFull") {
            trailerFullRef.current = result.isTrailerFull;
            setIsTrailerFull(result.isTrailerFull);
            persist(stopsRef.current, result.isTrailerFull, rest);
          } else {
            // deliverLoad succeeded: server already applied the bulk transition and cleared trailer-full.
            trailerFullRef.current = false;
            setIsTrailerFull(false);
            persist(stopsRef.current, false, rest);
          }
        } catch (err) {
          if (isNetworkError(err)) {
            setIsOffline(true);
            break; // still offline — stop draining, retry later
          }
          // Non-network failure (e.g. 409 from a stale state transition): drop the action rather than retry forever.
          queueRef.current = rest;
          setQueue(rest);
          persist(stopsRef.current, trailerFullRef.current, rest);
        }
      }
    } finally {
      draining.current = false;
    }
  }, [campaignId, persist]);

  useEffect(() => {
    poll();
    const interval = setInterval(poll, POLL_INTERVAL_MS);

    const onOnline = () => drainQueue();
    const onVisibility = () => {
      if (document.visibilityState === "visible") {
        poll();
        drainQueue();
      }
    };
    window.addEventListener("online", onOnline);
    document.addEventListener("visibilitychange", onVisibility);

    drainQueue();

    return () => {
      clearInterval(interval);
      window.removeEventListener("online", onOnline);
      document.removeEventListener("visibilitychange", onVisibility);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [campaignId, teamId]);

  const queueAction = useCallback(
    (stopId: string, type: QueuedStopActionType, reason?: string) => {
      const stop = stopsRef.current.find((s) => s.id === stopId);
      if (!stop || !isActionAllowed(stop.stopType, type)) return;

      const action: QueuedAction = {
        id: crypto.randomUUID(),
        scope: "stop",
        stopId,
        type,
        reason,
        queuedAt: new Date().toISOString(),
      };

      const nextQueue = [...queueRef.current, action];
      queueRef.current = nextQueue;
      setQueue(nextQueue);

      const nextStops = stopsRef.current.map((s) =>
        s.id === stopId
          ? { ...s, stopType: applyOptimisticTransition(s.stopType, type) }
          : s,
      );
      stopsRef.current = nextStops;
      setStops(nextStops);
      persist(nextStops, trailerFullRef.current, nextQueue);

      drainQueue();
    },
    [persist, drainQueue],
  );

  const queueTeamAction = useCallback(
    (type: QueuedTeamActionType) => {
      const action: QueuedAction = {
        id: crypto.randomUUID(),
        scope: "team",
        teamId,
        type,
        queuedAt: new Date().toISOString(),
      };

      const nextQueue = [...queueRef.current, action];
      queueRef.current = nextQueue;
      setQueue(nextQueue);

      if (type === "reportTrailerFull") {
        trailerFullRef.current = true;
        setIsTrailerFull(true);
        persist(stopsRef.current, true, nextQueue);
      } else {
        const nextStops = applyOptimisticDelivery(stopsRef.current);
        stopsRef.current = nextStops;
        trailerFullRef.current = false;
        setStops(nextStops);
        setIsTrailerFull(false);
        persist(nextStops, false, nextQueue);
      }

      drainQueue();
    },
    [persist, drainQueue, teamId],
  );

  return {
    stops,
    isTrailerFull,
    isOffline,
    pendingCount: queue.length,
    queueAction,
    queueTeamAction,
    refresh: poll,
  };
}
