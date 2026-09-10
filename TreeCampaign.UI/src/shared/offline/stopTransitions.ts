import type { Stop } from "../api/models/stop";
import type { QueuedStopActionType } from "./stopQueueStorage";

// Mirrors the subset of the backend's Stop state machine reachable from the team screen.
// Keep in sync with TreeCampaign.Domain's Assigned/Unresolved/Collected stop classes.
const ALLOWED_ACTIONS_BY_STOP_TYPE: Record<string, QueuedStopActionType[]> = {
  Assigned: ["collect", "unresolved"],
  Unresolved: ["retry"],
  Collected: ["correct"],
};

const RESULTING_STOP_TYPE: Record<QueuedStopActionType, string> = {
  collect: "Collected",
  unresolved: "Unresolved",
  retry: "Assigned",
  correct: "Assigned",
};

export function isActionAllowed(
  stopType: string,
  action: QueuedStopActionType,
): boolean {
  return ALLOWED_ACTIONS_BY_STOP_TYPE[stopType]?.includes(action) ?? false;
}

export function applyOptimisticTransition(
  stopType: string,
  action: QueuedStopActionType,
): string {
  return RESULTING_STOP_TYPE[action] ?? stopType;
}

// Mirrors DeliverLoadEndpoint: every Collected stop for the team becomes Delivered,
// and the team's trailer-full flag is cleared server-side as part of the same transaction.
export function applyOptimisticDelivery(stops: Stop[]): Stop[] {
  return stops.map((s) =>
    s.stopType === "Collected" ? { ...s, stopType: "Delivered" } : s,
  );
}
