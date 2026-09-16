import { useEffect, useState } from "react";
import {
  getCampaign,
  getNeighborhoods,
  getStops,
  getTeams,
} from "../../shared/api/client";
import type { Neighborhood } from "../../shared/api/models/neighborhood";
import type { Stop } from "../../shared/api/models/stop";
import type { Team, TeamKind, TeamStatus } from "../../shared/api/models/team";

export function useDispatchData(campaignId: string) {
  const [stops, setStops] = useState<Stop[]>([]);
  const [teams, setTeams] = useState<Team[]>([]);
  const [neighborhoods, setNeighborhoods] = useState<Neighborhood[]>([]);

  useEffect(() => {
    if (campaignId) {
      getStops(campaignId).then(setStops);
      getTeams(campaignId).then(setTeams);
      getCampaign(campaignId).then((campaign) => {
        if (campaign.territoryId) {
          getNeighborhoods(campaign.territoryId).then(setNeighborhoods);
        }
      });
    }
  }, [campaignId]);

  useEffect(() => {
    if (!campaignId) return;

    const es = new EventSource(`/api/${campaignId}/events`);

    es.addEventListener("campaign-update", (e: MessageEvent) => {
      const { type, data } = JSON.parse(e.data) as {
        type: string;
        data: Record<string, unknown>;
      };

      const patchTeamFunc = (
        teamId: string,
        patch: Record<string, unknown>,
      ) => {
        return () => {
          setTeams((prev) =>
            prev.map((t) => (t.id === teamId ? { ...t, ...patch } : t)),
          );
        };
      };

      const patchStopFunc = (stopId: string, patch: Partial<Stop>) => {
        return () => {
          setStops((prev) =>
            prev.map((s) => (s.id === stopId ? { ...s, ...patch } : s)),
          );
        };
      };

      const actionByEvent: Record<string, () => void> = {
        TeamCreated: () => {
          setTeams((prev) => [
            ...prev,
            {
              id: data.id as string,
              name: data.name as string,
              kind: data.kind as TeamKind,
              status: "Active" as TeamStatus,
              isTrailerFull: false,
              trailerSize: null,
              currentExtraTrees: 0,
              totalExtraTrees: 0,
              members: [],
            },
          ]);
        },
        TeamNameUpdated: patchTeamFunc(data.id as string, {
          name: data.name as string,
        }),
        TeamTrailerSizeUpdated: patchTeamFunc(data.id as string, {
          trailerSize: data.trailerSize as string | null,
        }),
        TeamWentOnBreak: patchTeamFunc(data.id as string, {
          status: "OnBreak",
        }),
        TeamResumedFromBreak: patchTeamFunc(data.id as string, {
          status: "Active",
        }),
        TeamReportedTrailerFull: patchTeamFunc(data.id as string, {
          isTrailerFull: true,
        }),
        TeamTrailerCleared: patchTeamFunc(data.id as string, {
          isTrailerFull: false,
        }),
        TeamExtraTreesAdjusted: patchTeamFunc(data.id as string, {
          currentExtraTrees: data.currentExtraTrees as number,
          totalExtraTrees: data.totalExtraTrees as number,
        }),
        TeamExtraTreesReset: patchTeamFunc(data.id as string, {
          currentExtraTrees: 0,
        }),
        StopCreated: () => {
          setStops((prev) => [
            ...prev,
            {
              id: data.id as string,
              address: data.address as Stop["address"],
              amount: data.amount as number,
              stopType: "Unassigned",
              assignedTeamId: undefined,
            },
          ]);
        },
        StopAssigned: patchStopFunc(data.id as string, {
          stopType: "Assigned",
          assignedTeamId: data.assignedTeamId as string,
        }),
        StopUnassigned: patchStopFunc(data.id as string, {
          stopType: "Unassigned",
          assignedTeamId: undefined,
        }),
        StopCollected: patchStopFunc(data.id as string, {
          stopType: "Collected",
        }),
        StopCollectionCorrected: patchStopFunc(data.id as string, {
          stopType: "Assigned",
        }),
        StopDelivered: patchStopFunc(data.id as string, {
          stopType: "Delivered",
        }),
        StopMarkedUnresolved: patchStopFunc(data.id as string, {
          stopType: "Unresolved",
        }),
        StopAbandoned: patchStopFunc(data.id as string, {
          stopType: "Abandoned",
        }),
        StopReassigned: patchStopFunc(data.id as string, {
          stopType: "Assigned",
          assignedTeamId: data.assignedTeamId as string,
        }),
        StopReopened: patchStopFunc(data.id as string, {
          stopType: "Unassigned",
          assignedTeamId: undefined,
        }),
        StopRetried: patchStopFunc(data.id as string, { stopType: "Assigned" }),
      };

      const action = actionByEvent[type];
      if (action !== undefined) {
        action();
      }
    });

    return () => es.close();
  }, [campaignId]);

  const updateStop = (updatedStop: Stop) => {
    setStops((prevStops) =>
      prevStops.map((s) => (s.id === updatedStop.id ? updatedStop : s)),
    );
  };

  const updateTeam = (updatedTeam: Team) => {
    setTeams((prev) =>
      prev.map((t) => (t.id === updatedTeam.id ? updatedTeam : t)),
    );
  };

  return { stops, teams, neighborhoods, updateStop, updateTeam };
}
