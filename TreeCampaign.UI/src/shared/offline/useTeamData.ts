import { useCallback, useEffect, useRef, useState } from "react";
import {
  addTeamMember,
  collectStop,
  correctStop,
  deliverLoad,
  getStopsForTeam,
  getTeam,
  markStopUnresolved,
  removeTeamMember,
  reportTrailerFull,
  retryStop,
  updateTeam,
} from "../api/client";
// HttpError (thrown by client.ts on non-2xx responses) is a real server reply and never
// treated as a connectivity failure — only a fetch-level TypeError means "offline."
import type { Stop } from "../api/models/stop";
import type { Team, TrailerSize } from "../api/models/team";
import {
  loadTeamStopsState,
  saveTeamStopsState,
  type QueuedAction,
  type QueuedStopActionType,
} from "./stopQueueStorage";
import {
  applyOptimisticAddMember,
  applyOptimisticDelivery,
  applyOptimisticRemoveMember,
  applyOptimisticTeamUpdate,
  applyOptimisticTransition,
  isActionAllowed,
} from "./stopTransitions";

const POLL_INTERVAL_MS = 30_000;

type ActionResult =
  | { kind: "stop"; stop: Stop }
  | { kind: "team"; team: Team }
  | { kind: "delivered" };

async function runAction(
  campaignId: string,
  action: QueuedAction,
): Promise<ActionResult> {
  if (action.scope === "team") {
    switch (action.type) {
      case "reportTrailerFull":
        return { kind: "team", team: await reportTrailerFull(campaignId, action.teamId) };
      case "deliverLoad":
        await deliverLoad(campaignId, action.teamId);
        return { kind: "delivered" };
      case "updateTeam":
        return {
          kind: "team",
          team: await updateTeam(campaignId, action.teamId, action.name, action.trailerSize),
        };
      case "addMember":
        return {
          kind: "team",
          team: await addTeamMember(
            campaignId,
            action.teamId,
            action.name,
            action.phoneNumber,
            action.scoutRelativeName,
          ),
        };
      case "removeMember":
        return {
          kind: "team",
          team: await removeTeamMember(campaignId, action.teamId, action.memberId),
        };
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

export function useTeamData(campaignId: string, teamId: string) {
  const initial = loadTeamStopsState(campaignId, teamId);
  const [stops, setStops] = useState<Stop[]>(initial.lastKnownStops);
  const [team, setTeam] = useState<Team | null>(initial.lastKnownTeam);
  const [queue, setQueue] = useState<QueuedAction[]>(initial.queue);
  const [isOffline, setIsOffline] = useState(false);
  const draining = useRef(false);

  const stopsRef = useRef(stops);
  stopsRef.current = stops;
  const teamRef = useRef(team);
  teamRef.current = team;
  const queueRef = useRef(queue);
  queueRef.current = queue;

  const persist = useCallback(
    (nextStops: Stop[], nextTeam: Team | null, nextQueue: QueuedAction[]) => {
      saveTeamStopsState(campaignId, teamId, {
        lastKnownStops: nextStops,
        lastKnownTeam: nextTeam,
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
      // While a team action is still queued, our optimistic local team is more current
      // than whatever the server last reported — keep it until the action actually lands.
      const mergedTeam = hasPendingTeamAction() ? teamRef.current : serverTeam;

      setStops(mergedStops);
      setTeam(mergedTeam);
      persist(mergedStops, mergedTeam, queueRef.current);
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
            persist(merged, teamRef.current, rest);
          } else if (result.kind === "team") {
            teamRef.current = result.team;
            setTeam(result.team);
            persist(stopsRef.current, result.team, rest);
          } else {
            // deliverLoad succeeded: server already applied the bulk transition and cleared trailer-full.
            const nextTeam = teamRef.current
              ? { ...teamRef.current, isTrailerFull: false }
              : teamRef.current;
            teamRef.current = nextTeam;
            setTeam(nextTeam);
            persist(stopsRef.current, nextTeam, rest);
          }
        } catch (err) {
          if (isNetworkError(err)) {
            setIsOffline(true);
            break; // still offline — stop draining, retry later
          }
          // Non-network failure (e.g. 409 from a stale state transition): drop the action rather than retry forever.
          queueRef.current = rest;
          setQueue(rest);
          persist(stopsRef.current, teamRef.current, rest);
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

  const enqueue = useCallback(
    (action: QueuedAction, applyOptimistic: () => void) => {
      const nextQueue = [...queueRef.current, action];
      queueRef.current = nextQueue;
      setQueue(nextQueue);
      applyOptimistic();
      persist(stopsRef.current, teamRef.current, nextQueue);
      drainQueue();
    },
    [persist, drainQueue],
  );

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

      enqueue(action, () => {
        const nextStops = stopsRef.current.map((s) =>
          s.id === stopId
            ? { ...s, stopType: applyOptimisticTransition(s.stopType, type) }
            : s,
        );
        stopsRef.current = nextStops;
        setStops(nextStops);
      });
    },
    [enqueue],
  );

  const queueTeamAction = useCallback(
    (type: "reportTrailerFull" | "deliverLoad") => {
      const action: QueuedAction = {
        id: crypto.randomUUID(),
        scope: "team",
        teamId,
        type,
        queuedAt: new Date().toISOString(),
      };

      enqueue(action, () => {
        if (!teamRef.current) return;
        if (type === "reportTrailerFull") {
          const nextTeam = { ...teamRef.current, isTrailerFull: true };
          teamRef.current = nextTeam;
          setTeam(nextTeam);
        } else {
          const nextStops = applyOptimisticDelivery(stopsRef.current);
          stopsRef.current = nextStops;
          setStops(nextStops);
          const nextTeam = { ...teamRef.current, isTrailerFull: false };
          teamRef.current = nextTeam;
          setTeam(nextTeam);
        }
      });
    },
    [enqueue, teamId],
  );

  const queueUpdateTeam = useCallback(
    (name: string, trailerSize?: TrailerSize) => {
      const action: QueuedAction = {
        id: crypto.randomUUID(),
        scope: "team",
        teamId,
        type: "updateTeam",
        name,
        trailerSize,
        queuedAt: new Date().toISOString(),
      };

      enqueue(action, () => {
        if (!teamRef.current) return;
        const nextTeam = applyOptimisticTeamUpdate(teamRef.current, action);
        teamRef.current = nextTeam;
        setTeam(nextTeam);
      });
    },
    [enqueue, teamId],
  );

  const queueAddMember = useCallback(
    (name: string, phoneNumber?: string, scoutRelativeName?: string) => {
      const action: QueuedAction = {
        id: crypto.randomUUID(),
        scope: "team",
        teamId,
        type: "addMember",
        tempMemberId: crypto.randomUUID(),
        name,
        phoneNumber,
        scoutRelativeName,
        queuedAt: new Date().toISOString(),
      };

      enqueue(action, () => {
        if (!teamRef.current) return;
        const nextTeam = applyOptimisticAddMember(teamRef.current, action);
        teamRef.current = nextTeam;
        setTeam(nextTeam);
      });
    },
    [enqueue, teamId],
  );

  const queueRemoveMember = useCallback(
    (memberId: string) => {
      // If the member being removed is still only a queued, not-yet-sent addMember
      // (a temp id), just drop that queued add — there's nothing on the server to remove yet.
      const pendingAdd = queueRef.current.find(
        (a): a is Extract<QueuedAction, { type: "addMember" }> =>
          a.scope === "team" && a.type === "addMember" && a.tempMemberId === memberId,
      );
      if (pendingAdd) {
        const nextQueue = queueRef.current.filter((a) => a.id !== pendingAdd.id);
        queueRef.current = nextQueue;
        setQueue(nextQueue);
        if (teamRef.current) {
          const nextTeam = {
            ...teamRef.current,
            members: teamRef.current.members.filter((m) => m.id !== memberId),
          };
          teamRef.current = nextTeam;
          setTeam(nextTeam);
          persist(stopsRef.current, nextTeam, nextQueue);
        }
        return;
      }

      const action: QueuedAction = {
        id: crypto.randomUUID(),
        scope: "team",
        teamId,
        type: "removeMember",
        memberId,
        queuedAt: new Date().toISOString(),
      };

      enqueue(action, () => {
        if (!teamRef.current) return;
        const nextTeam = applyOptimisticRemoveMember(teamRef.current, action);
        teamRef.current = nextTeam;
        setTeam(nextTeam);
      });
    },
    [enqueue, persist, teamId],
  );

  return {
    stops,
    team,
    isOffline,
    pendingCount: queue.length,
    queueAction,
    queueTeamAction,
    queueUpdateTeam,
    queueAddMember,
    queueRemoveMember,
    refresh: poll,
  };
}
