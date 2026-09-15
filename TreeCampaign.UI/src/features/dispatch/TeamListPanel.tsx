import { useState } from "react";
import Button from "../../components/Button";
import CreateTeamForm from "../teams/CreateTeamForm";
import type { Stop } from "../../shared/api/models/stop";
import type { Team } from "../../shared/api/models/team";
import TeamCard from "./TeamCard";

interface TeamListPanelProps {
  campaignId: string;
  sortedTeams: Team[];
  stops: Stop[];
  assignMode: boolean;
  teamExceedsSelection: (team: Team) => boolean;
  onTeamClick: (team: Team) => void;
  onUpdateStop: (stop: Stop) => void;
  onUpdateTeam: (team: Team) => void;
}

export default function TeamListPanel({
  campaignId,
  sortedTeams,
  stops,
  assignMode,
  teamExceedsSelection,
  onTeamClick,
  onUpdateStop,
  onUpdateTeam,
}: TeamListPanelProps) {
  const [showCreateTeam, setShowCreateTeam] = useState(false);

  return (
    <div className="w-4/12 flex flex-col gap-2 border border-gray-300 rounded-lg p-4">
      <div className="flex items-center justify-between h-11">
        <h2 className="text-base font-semibold">Hold</h2>
        <Button onClick={() => setShowCreateTeam((v) => !v)}>
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
      {sortedTeams.map((team) => (
        <TeamCard
          key={team.id}
          campaignId={campaignId}
          team={team}
          stops={stops.filter(
            (stop) =>
              stop.assignedTeamId === team.id && stop.stopType !== "Delivered",
          )}
          assignMode={assignMode}
          blocked={assignMode && teamExceedsSelection(team)}
          onClick={onTeamClick}
          onUpdateStop={onUpdateStop}
          onUpdateTeam={onUpdateTeam}
        />
      ))}
    </div>
  );
}
