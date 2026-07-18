import { useParams } from "react-router-dom";
import {
  assignStopToTeam,
  getCampaign,
  getNeighborhoods,
  getStops,
  getTeams,
} from "../../shared/api/client";
import { useEffect, useState } from "react";
import type { Neighborhood } from "../../shared/api/models/neighborhood";
import type { Team, TeamKind, TeamStatus } from "../../shared/api/models/team";
import type { Stop } from "../../shared/api/models/stop";
import TeamCard from "./TeamCard";
import StopCard from "./StopCard";
import CreateTeamForm from "../teams/CreateTeamForm";
import NeighborhoodSection from "./NeighborhoodSection";
import { MagnifyingGlassIcon } from "@heroicons/react/24/outline";
import NavigationPage from "../../shared/components/NavigationPage";
import Button from "../../components/Button";

function parseHouseNumber(displayName: string): number {
  const lastToken = displayName.split(",")[0].split(" ").pop() ?? "";
  return parseInt(lastToken, 10) || 0;
}

export default function DispatchScreen() {
  const params = useParams();
  const campaignId = params.campaignId!;
  const [stops, setStops] = useState<Stop[]>([]);
  const [teams, setTeams] = useState<Team[]>([]);
  const [neighborhoods, setNeighborhoods] = useState<Neighborhood[]>([]);
  const [selectedStopIds, setSelectedStopIds] = useState<Set<string>>(
    new Set(),
  );
  const [showCreateTeam, setShowCreateTeam] = useState(false);
  const [onlyUnassigned, setOnlyUnassigned] = useState(true);
  const [filter, setFilter] = useState("");

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
      const { type, data } = JSON.parse(e.data) as { type: string; data: Record<string, unknown> };

      const patchTeamFunc = (teamId: string, patch: Record<string, unknown>) => {
        return () => {
          setTeams((prev) =>
            prev.map((t) => (t.id === teamId ? { ...t, ...patch } : t)),
          );
        }
      }

      const patchStopFunc = (stopId: string, patch: Partial<Stop>) => {
        return () => {
          setStops((prev) =>
            prev.map((s) => (s.id === stopId ? { ...s, ...patch } : s)),
          );
        }
      }

      const actionByEvent: Record<string, () => void> = {
        TeamCreated: () => {
          setTeams((prev) => [...prev, {
            id: data.id as string,
            name: data.name as string,
            kind: data.kind as TeamKind,
            status: "Active" as TeamStatus,
            isTrailerFull: false,
            trailerSize: null,
            members: []
          }]);
        },
        TeamNameUpdated: patchTeamFunc(data.id as string, { name: data.name as string }),
        TeamTrailerSizeUpdated: patchTeamFunc(data.id as string, { trailerSize: data.trailerSize as string | null }),
        TeamWentOnBreak: patchTeamFunc(data.id as string, { status: "OnBreak" }),
        TeamResumedFromBreak: patchTeamFunc(data.id as string, { status: "Active" }),
        TeamReportedTrailerFull: patchTeamFunc(data.id as string, { isTrailerFull: true }),
        TeamTrailerCleared: patchTeamFunc(data.id as string, { isTrailerFull: false }),
        StopCreated: () => {
          setStops((prev) => [...prev, {
            id: data.id as string,
            address: data.address as Stop["address"],
            amount: data.amount as number,
            stopType: "Unassigned",
            assignedTeamId: undefined,
          }]);
        },
        StopAssigned: patchStopFunc(data.id as string, {
          stopType: "Assigned",
          assignedTeamId: data.assignedTeamId as string,
        }),
        StopUnassigned: patchStopFunc(data.id as string, {
          stopType: "Unassigned",
          assignedTeamId: undefined,
        }),
        StopCollected: patchStopFunc(data.id as string, { stopType: "Collected" }),
        StopCollectionCorrected: patchStopFunc(data.id as string, { stopType: "Assigned" }),
        StopDelivered: patchStopFunc(data.id as string, { stopType: "Delivered" }),
        StopMarkedUnresolved: patchStopFunc(data.id as string, { stopType: "Unresolved" }),
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

  const streetSections = neighborhoods.flatMap((n) => n.streetSections);
  const sectionById = new Map(streetSections.map((s) => [s.id, s]));

  const filteredStops =
    onlyUnassigned || filter !== ""
      ? stops.filter(
          (s) =>
            (!onlyUnassigned || s.stopType === "Unassigned") &&
            s.address.displayName
              .toLocaleLowerCase()
              .startsWith(filter.toLocaleLowerCase()),
        )
      : stops;

  const sortedStops = filteredStops.sort((a, b) => {
    const sA = sectionById.get(a.address.streetSectionId);
    const sB = sectionById.get(b.address.streetSectionId);
    if (!sA && !sB) return 0;
    if (!sA) return 1;
    if (!sB) return -1;
    if (sA.sortOrder !== sB.sortOrder) return sA.sortOrder - sB.sortOrder;
    const hA = parseHouseNumber(a.address.displayName);
    const hB = parseHouseNumber(b.address.displayName);
    return sA.direction === 0 ? hA - hB : hB - hA;
  });

  const stopsByNeighborhood = neighborhoods
    .map((n) => ({
      neighborhood: n,
      stops: sortedStops.filter(
        (stop) =>
          sectionById.get(stop.address.streetSectionId)?.neighborhoodId ===
          n.id,
      ),
    }))
    .filter((group) => group.stops.length > 0);

  const ungroupedStops = sortedStops.filter(
    (stop) => !sectionById.has(stop.address.streetSectionId),
  );

  const toggleStop = (stopId: string) => {
    setSelectedStopIds((prev) => {
      const newSet = new Set(prev);
      if (newSet.has(stopId)) {
        newSet.delete(stopId);
      } else {
        newSet.add(stopId);
      }
      return newSet;
    });
  };

  const updateStop = (updatedStop: Stop) => {
    setStops((prevStops) =>
      prevStops.map((s) => (s.id === updatedStop.id ? updatedStop : s)),
    );
  };

  const updateTeam = (updatedTeam: Team) => {
    setTeams((prev) => prev.map((t) => (t.id === updatedTeam.id ? updatedTeam : t)));
  };

  const clickTeam = (team: Team) => {
    selectedStopIds.forEach((stopId) => {
      assignStopToTeam(campaignId!, stopId, team.id).then(updateStop);
    });
    setSelectedStopIds(new Set());
  };

  return (
    <NavigationPage>
      <div>
        <h1 className="text-xl font-bold">Dispatch</h1>
        <div className="flex gap-2 mt-4">
          <div className="w-9/12 flex flex-col gap-2">
            <div className="flex gap-2">
              <div className="flex border rounded-sm p-2 border-gray-200 w-6/12 items-center">
                <MagnifyingGlassIcon className="h-5 w-5 mr-2" />
                <input
                  type="text"
                  value={filter}
                  className="w-full text-gray-500 focus:outline-0"
                  placeholder="Søg efter stop"
                  onChange={(e) => setFilter(e.target.value)}
                />
              </div>
              <label htmlFor="onlyUnassigned" className="flex items-center">
                <input
                  type="checkbox"
                  id="onlyUnassigned"
                  className="w-4 h-4 mr-2"
                  checked={onlyUnassigned}
                  onChange={(e) => setOnlyUnassigned(e.target.checked)}
                />
                Vis kun frie stop
              </label>
            </div>
            <h2 className="text-base font-semibold">
              {(onlyUnassigned ? "Frie" : "Alle") +
                (filter.length > 0
                  ? ` stop, der starter med '${filter}' `
                  : " stop ")}
              - {sortedStops.length} stop
            </h2>
            {stopsByNeighborhood.map(({ neighborhood, stops: nStops }) => (
              <NeighborhoodSection
                key={neighborhood.id}
                name={neighborhood.name}
                stops={nStops}
                campaignId={campaignId}
                selectedStopIds={selectedStopIds}
                toggleStop={toggleStop}
              />
            ))}
            {ungroupedStops.map((stop) => (
              <StopCard
                key={stop.id}
                campaignId={campaignId}
                stop={stop}
                assignMode={true}
                selected={selectedStopIds.has(stop.id)}
                onToggleSelect={toggleStop}
              />
            ))}
            {stopsByNeighborhood.length === 0 &&
              ungroupedStops.length === 0 && (
                <div className="w-full bg-gray-50 rounded-sm p-4 text-center text-sm text-gray-600">
                  Ingen stop
                </div>
              )}
          </div>
          <div className="w-3/12 flex flex-col gap-2">
            <div className="flex items-center justify-between">
              <h2 className="text-base font-semibold">Hold</h2>
              <Button
                onClick={() => setShowCreateTeam((v) => !v)}
                className="bg-blue-600"
              >
                {showCreateTeam ? "Annuller" : "Nyt hold"}
              </Button>
            </div>
            {showCreateTeam && (
              <CreateTeamForm
                campaignId={campaignId}
                onCreated={() => setShowCreateTeam(false)}
                onCancel={() => setShowCreateTeam(false)}
              />
            )}
            {teams.map((team: Team) => (
              <TeamCard
                key={team.id}
                campaignId={campaignId}
                team={team}
                stops={stops.filter((stop) => stop.assignedTeamId === team.id)}
                assignMode={selectedStopIds.size > 0}
                onClick={clickTeam}
                onUpdateStop={updateStop}
                onUpdateTeam={updateTeam}
              />
            ))}
          </div>
        </div>
      </div>
    </NavigationPage>
  );
}
