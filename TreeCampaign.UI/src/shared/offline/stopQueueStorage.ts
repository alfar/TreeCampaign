import type { Stop } from "../api/models/stop";

export type QueuedStopActionType =
  | "collect"
  | "unresolved"
  | "retry"
  | "correct";

export type QueuedTeamActionType = "reportTrailerFull" | "deliverLoad";

export interface QueuedStopAction {
  id: string;
  scope: "stop";
  stopId: string;
  type: QueuedStopActionType;
  reason?: string;
  queuedAt: string;
}

export interface QueuedTeamAction {
  id: string;
  scope: "team";
  teamId: string;
  type: QueuedTeamActionType;
  queuedAt: string;
}

export type QueuedAction = QueuedStopAction | QueuedTeamAction;

interface TeamStopsState {
  lastKnownStops: Stop[];
  isTrailerFull: boolean | null;
  queue: QueuedAction[];
}

const EMPTY_STATE: TeamStopsState = {
  lastKnownStops: [],
  isTrailerFull: null,
  queue: [],
};

function storageKey(campaignId: string, teamId: string): string {
  return `treecampaign:team-stops:${campaignId}:${teamId}`;
}

export function loadTeamStopsState(
  campaignId: string,
  teamId: string,
): TeamStopsState {
  try {
    const raw = localStorage.getItem(storageKey(campaignId, teamId));
    if (!raw) return EMPTY_STATE;
    const parsed = JSON.parse(raw);
    return {
      lastKnownStops: parsed.lastKnownStops ?? [],
      isTrailerFull: parsed.isTrailerFull ?? null,
      queue: parsed.queue ?? [],
    };
  } catch {
    return EMPTY_STATE;
  }
}

export function saveTeamStopsState(
  campaignId: string,
  teamId: string,
  state: TeamStopsState,
): void {
  try {
    localStorage.setItem(
      storageKey(campaignId, teamId),
      JSON.stringify(state),
    );
  } catch {
    // Storage full or unavailable (private browsing) — queue keeps working in memory for this session.
  }
}

export type { TeamStopsState };
