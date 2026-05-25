import {
  CheckIcon,
  ExclamationTriangleIcon,
} from "@heroicons/react/24/outline";
import type { Stop } from "../../shared/api/models/stop";
import type { Team } from "../../shared/api/models/team";
import { useState } from "react";
import StopCard from "./StopCard";

interface TeamCardProps {
  campaignId: string;
  team: Team;
  stops: Stop[];
  assignMode: boolean;
  onClick: (team: Team) => any;
  onUpdateStop?: (stop: Stop) => any;
}

export default function TeamCard({
  campaignId,
  team,
  stops,
  assignMode,
  onClick,
  onUpdateStop,
}: TeamCardProps) {
  const [expanded, setExpanded] = useState(false);
  const counts = stops.reduce(
    (acc, stop) => {
      switch (stop.stopType) {
        case "Assigned":
          acc.assigned += 1;
          break;
        case "Unresolved":
          acc.unresolved += 1;
          break;
        case "Collected":
          acc.collected += 1;
          break;
      }
      return acc;
    },
    { assigned: 0, unresolved: 0, collected: 0 },
  );


  return (
    <div
      className={
        assignMode
          ? "p-4 border border-amber-400 rounded cursor-pointer"
          : "p-4 border rounded"
      }
      onClick={() => onClick(team)}
    >
      <h2 className="text-lg font-semibold">{team.name}</h2>
      <p onClick={() => setExpanded((prev) => !prev)}>
        {counts.assigned > 0 && (
          <span>
            <span className="text-blue-500">{counts.assigned}</span> /{" "}
          </span>
        )}
        {counts.unresolved > 0 && (
          <span>
            <ExclamationTriangleIcon className="h-5 w-5 text-red-500 inline" />{" "}
            <span className="text-red-500">{counts.unresolved}</span> /{" "}
          </span>
        )}
        {counts.collected > 0 && (
          <span>
            <CheckIcon className="h-5 w-5 text-green-500 inline" />{" "}
            <span className="text-green-500">{counts.collected}</span> /{" "}
          </span>
        )}
        {stops.length} stops
      </p>
      {expanded && (
        <div className="mt-2 flex flex-col gap-2">
          {stops.map((stop) => (
            <StopCard 
                  key={stop.id}
                  campaignId={campaignId}
                  stop={stop}
                  assignMode={false}
                  onUpdateStop={onUpdateStop} />
          ))}
        </div>
      )}
    </div>
  );
}
