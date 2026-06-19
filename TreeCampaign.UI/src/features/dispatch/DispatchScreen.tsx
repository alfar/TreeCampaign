import { useParams } from "react-router-dom";
import { assignStopToTeam, getStops, getTeams } from "../../shared/api/client";
import { useEffect, useState } from "react";
import type { Team } from "../../shared/api/models/team";
import type { Stop } from "../../shared/api/models/stop";
import TeamCard from "./TeamCard";
import StopCard from "./StopCard";
import CreateTeamForm from "../teams/CreateTeamForm";

export default function DispatchScreen() {
  const params = useParams();
  const campaignId = params.campaignId!;
  const [stops, setStops] = useState<Stop[]>([]);
  const [teams, setTeams] = useState<Team[]>([]);
  const [selectedStopIds, setSelectedStopIds] = useState<Set<string>>(new Set());
  const [showCreateTeam, setShowCreateTeam] = useState(false);

  useEffect(() => {
    if (campaignId) {
      getStops(campaignId).then(setStops);
      getTeams(campaignId).then(setTeams);
    }
  }, [campaignId]);

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
      <div className="space-y-4">
        {stops
          .filter((stop) => stop.stopType === "Unassigned")
          .map((stop) => (
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
      <div className="space-y-4 mt-4">
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
            onCreated={(team) => { setTeams((prev) => [...prev, team]); setShowCreateTeam(false); }}
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
  );
}
