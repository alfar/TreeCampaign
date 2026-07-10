import type { Stop } from "../../shared/api/models/stop";
import type { Team } from "../../shared/api/models/team";
import { useState } from "react";
import StopCard from "./StopCard";
import ProgressBar from "../../components/ProgressBar";
import { Link } from "react-router-dom";
import { QrCodeIcon } from "@heroicons/react/24/outline";
import { sendTeamOnBreak } from "../../shared/api/client";

interface TeamCardProps {
  campaignId: string;
  team: Team;
  stops: Stop[];
  assignMode: boolean;
  onClick: (team: Team) => any;
  onUpdateStop?: (stop: Stop) => any;
  onUpdateTeam?: (team: Team) => any;
}

export default function TeamCard({
  campaignId,
  team,
  stops,
  assignMode,
  onClick,
  onUpdateStop,
  onUpdateTeam,
}: TeamCardProps) {
  const [expanded, setExpanded] = useState(false);
  const counts = stops.reduce(
    (acc, stop) => {
      acc.total += stop.amount;
      switch (stop.stopType) {
        case "Assigned":
          acc.assigned += stop.amount;
          break;
        case "Unresolved":
          acc.unresolved += stop.amount;
          break;
        case "Collected":
          acc.collected += stop.amount;
          break;
      }
      return acc;
    },
    { assigned: 0, unresolved: 0, collected: 0, total: 0 },
  );

  const statusBadge = team.status === 'OnBreak'
    ? <span className="text-xs bg-yellow-100 text-yellow-800 px-2 py-0.5 rounded-full">Pause</span>
    : team.kind === 'Trailer' && team.isTrailerFull
    ? <span className="text-xs bg-orange-100 text-orange-800 px-2 py-0.5 rounded-full">Trailer fuld</span>
    : null;

  const handleBreak = (e: React.MouseEvent) => {
    e.stopPropagation();
    sendTeamOnBreak(campaignId, team.id).then((updated) => onUpdateTeam?.(updated));
  };

  return (
    <div
      className={
        assignMode
          ? "p-4 border border-amber-400 rounded-sm cursor-pointer"
          : team.status !== 'Active'
          ? "p-4 border border-gray-200 rounded-sm opacity-60"
          : "p-4 border border-gray-200 rounded-sm"
      }
      onClick={() => onClick(team)}
    >
      <h2 className="text-lg font-semibold flex justify-between gap-2 flex-wrap">
        <span className="flex items-center gap-2">{team.name}{statusBadge}</span>
        <span className="flex items-center gap-2">
          {team.status === 'Active' && (
            <button
              className="text-xs text-gray-500 border border-gray-300 px-2 py-0.5 rounded hover:bg-gray-50"
              onClick={handleBreak}
            >
              Pause
            </button>
          )}
          <Link
            className="inline"
            to={"/campaigns/" + campaignId + "/teams/" + team.id + "/info"}
            target="_blank"
            onClick={(e) => e.stopPropagation()}
          >
            <QrCodeIcon className="w-6 h-6" />
          </Link>
        </span>
      </h2>
      <ProgressBar
        parts={[
          { title: "Opsamlet", amount: counts.collected, color: "#006600" },
          { title: "Fejlet", amount: counts.unresolved, color: "#ff0000" },
        ]}
        total={counts.total}
        onClick={() => setExpanded(!expanded)}
      />
      {expanded && (
        <div className="mt-2 flex flex-col gap-2">
          {stops.map((stop) => (
            <StopCard
              key={stop.id}
              campaignId={campaignId}
              stop={stop}
              assignMode={false}
              onUpdateStop={onUpdateStop}
            />
          ))}
        </div>
      )}
    </div>
  );
}
