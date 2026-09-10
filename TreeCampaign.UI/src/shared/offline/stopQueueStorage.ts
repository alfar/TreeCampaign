import type { Stop } from "../api/models/stop";
import type { Team, TrailerSize } from "../api/models/team";

export type QueuedStopActionType =
  | "collect"
  | "unresolved"
  | "retry"
  | "correct";

export interface QueuedStopAction {
  id: string;
  scope: "stop";
  stopId: string;
  type: QueuedStopActionType;
  reason?: string;
  queuedAt: string;
}

export interface QueuedReportTrailerFullAction {
  id: string;
  scope: "team";
  teamId: string;
  type: "reportTrailerFull";
  queuedAt: string;
}

export interface QueuedDeliverLoadAction {
  id: string;
  scope: "team";
  teamId: string;
  type: "deliverLoad";
  queuedAt: string;
}

export interface QueuedUpdateTeamAction {
  id: string;
  scope: "team";
  teamId: string;
  type: "updateTeam";
  name: string;
  trailerSize?: TrailerSize;
  queuedAt: string;
}

export interface QueuedAddMemberAction {
  id: string;
  scope: "team";
  teamId: string;
  type: "addMember";
  tempMemberId: string;
  name: string;
  phoneNumber?: string;
  scoutRelativeName?: string;
  queuedAt: string;
}

export interface QueuedRemoveMemberAction {
  id: string;
  scope: "team";
  teamId: string;
  type: "removeMember";
  memberId: string;
  queuedAt: string;
}

export type QueuedTeamAction =
  | QueuedReportTrailerFullAction
  | QueuedDeliverLoadAction
  | QueuedUpdateTeamAction
  | QueuedAddMemberAction
  | QueuedRemoveMemberAction;

export type QueuedTeamActionType = QueuedTeamAction["type"];

export type QueuedAction = QueuedStopAction | QueuedTeamAction;

interface TeamStopsState {
  lastKnownStops: Stop[];
  lastKnownTeam: Team | null;
  queue: QueuedAction[];
}

const EMPTY_STATE: TeamStopsState = {
  lastKnownStops: [],
  lastKnownTeam: null,
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
      lastKnownTeam: parsed.lastKnownTeam ?? null,
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
