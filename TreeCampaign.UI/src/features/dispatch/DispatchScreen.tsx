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
import type { Team } from "../../shared/api/models/team";
import type { Stop } from "../../shared/api/models/stop";
import TeamCard from "./TeamCard";
import StopCard from "./StopCard";
import CreateTeamForm from "../teams/CreateTeamForm";

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

  const streetSections = neighborhoods.flatMap((n) => n.streetSections);
  const sectionById = new Map(streetSections.map((s) => [s.id, s]));

  const sortedUnassignedStops = stops
    .filter((stop) => stop.stopType === "Unassigned")
    .sort((a, b) => {
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
      stops: sortedUnassignedStops.filter(
        (stop) =>
          sectionById.get(stop.address.streetSectionId)?.neighborhoodId ===
          n.id,
      ),
    }))
    .filter((group) => group.stops.length > 0);

  const ungroupedStops = sortedUnassignedStops.filter(
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

  const clickTeam = (team: Team) => {
    selectedStopIds.forEach((stopId) => {
      assignStopToTeam(campaignId!, stopId, team.id).then(updateStop);
    });
    setSelectedStopIds(new Set());
  };

  return (
    <div className="p-4 text-lg">
      <h1 className="text-xl font-bold">Dispatch</h1>
      <div className="flex gap-2">
        <div className="w-9/12">
          {stopsByNeighborhood.map(({ neighborhood, stops: nStops }) => (
            <div key={neighborhood.id}>
              <h2 className="text-base font-semibold mb-2">
                {neighborhood.name}
              </h2>
              <div className="space-y-2">
                {nStops.map((stop) => (
                  <StopCard
                    key={stop.id}
                    campaignId={campaignId}
                    stop={stop}
                    assignMode={true}
                    selected={selectedStopIds.has(stop.id)}
                    onToggleSelect={toggleStop}
                  />
                ))}
              </div>
            </div>
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
        </div>
        <div className="w-3/12 flex flex-col gap-2">
          <div className="flex items-center justify-between">
            <h2 className="text-base font-semibold">Hold</h2>
            <button
              onClick={() => setShowCreateTeam((v) => !v)}
              className="text-sm bg-blue-600 text-white py-1 px-3 rounded"
            >
              {showCreateTeam ? "Annuller" : "Nyt hold"}
            </button>
          </div>
          {showCreateTeam && (
            <CreateTeamForm
              campaignId={campaignId}
              onCreated={(team) => {
                setTeams((prev) => [...prev, team]);
                setShowCreateTeam(false);
              }}
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
            />
          ))}
        </div>
      </div>
    </div>
  );
}
