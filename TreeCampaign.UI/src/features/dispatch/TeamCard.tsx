import type { Stop } from "../../shared/api/models/stop";
import { trailerCapacity, trailerSizeLabels, type Team } from "../../shared/api/models/team";
import { useState } from "react";
import StopCard from "./StopCard";
import ProgressBar from "../../components/ProgressBar";
import { Link } from "react-router-dom";
import { QrCodeIcon, Bars2Icon, Bars3Icon, Bars4Icon, UsersIcon } from "@heroicons/react/24/outline";
import { clearTrailerFull, sendTeamOnBreak } from "../../shared/api/client";

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
        case "Delivered":
          acc.delivered += stop.amount;
          break;
      }
      return acc;
    },
    { assigned: 0, unresolved: 0, collected: 0, delivered: 0, total: 0 },
  );

  const estimatedCapacity =
    team.kind === "Trailer" && team.trailerSize ? trailerCapacity[team.trailerSize] : null;
  const estimatedLoad = counts.assigned + counts.collected;
  const remainingRoom = estimatedCapacity !== null ? estimatedCapacity - estimatedLoad : null;
  const isNearOrOverCapacity = remainingRoom !== null && remainingRoom <= 0;

  const trailerSizeIcon: Record<NonNullable<Team["trailerSize"]>, typeof Bars2Icon> = {
    Small: Bars2Icon,
    Large: Bars3Icon,
    Boogie: Bars4Icon,
  };
  const TeamKindIcon =
    team.kind === "Trailer" && team.trailerSize ? trailerSizeIcon[team.trailerSize] : UsersIcon;

  const statusBadge = team.status === 'OnBreak'
    ? <span className="text-xs bg-yellow-100 text-yellow-800 px-2 py-0.5 rounded-full">Pause</span>
    : team.kind === 'Trailer' && team.isTrailerFull
    ? <span className="text-xs bg-orange-100 text-orange-800 px-2 py-0.5 rounded-full">Trailer fuld</span>
    : null;

  const handleBreak = (e: React.MouseEvent) => {
    e.stopPropagation();
    sendTeamOnBreak(campaignId, team.id).then((updated) => onUpdateTeam?.(updated));
  };

  const handleClearTrailerFull = (e: React.MouseEvent) => {
    e.stopPropagation();
    clearTrailerFull(campaignId, team.id).then((updated) => onUpdateTeam?.(updated));
  };

  return (
    <div
      className={
        assignMode
          ? "p-4 border border-amber-400 rounded-sm cursor-pointer"
          : team.kind === 'Trailer' && team.isTrailerFull
          ? "p-4 border border-amber-400 bg-amber-50 rounded-sm"
          : team.status !== 'Active'
          ? "p-4 border border-gray-200 rounded-sm opacity-60"
          : "p-4 border border-gray-200 rounded-sm"
      }
      onClick={() => onClick(team)}
    >
      <h2 className="text-lg font-semibold flex justify-between gap-2 flex-wrap">
        <span className="flex items-center gap-2">
          <TeamKindIcon className="w-5 h-5 text-gray-500 shrink-0" />
          {team.name}
          {team.kind === "Trailer" && team.trailerSize && (
            <span className="text-xs font-normal text-gray-500">{trailerSizeLabels[team.trailerSize]}</span>
          )}
          {remainingRoom !== null && !team.isTrailerFull && (
            <span
              className={
                isNearOrOverCapacity
                  ? "text-xs font-normal text-amber-700"
                  : "text-xs font-normal text-gray-500"
              }
            >
              {isNearOrOverCapacity
                ? `Fuld (est. ${estimatedLoad}/${estimatedCapacity})`
                : `Plads til ~${remainingRoom} flere træer`}
            </span>
          )}
          {statusBadge}
        </span>
        <span className="flex items-center gap-2">
          {team.kind === 'Trailer' && team.isTrailerFull && (
            <button
              className="text-xs text-amber-800 border border-amber-400 px-2 py-0.5 rounded hover:bg-amber-100"
              onClick={handleClearTrailerFull}
            >
              Nulstil trailer fuld
            </button>
          )}
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
        total={counts.total - counts.delivered}
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
