import {
  BuildingOffice2Icon,
  ChevronDownIcon,
  ChevronUpIcon,
} from "@heroicons/react/24/outline";
import { useState } from "react";
import type { Stop } from "../../shared/api/models/stop";
import StopCard from "./StopCard";

interface NeighborhoodSectionProps {
  name: string;
  stops: Stop[];
  selectedStopIds: Set<string>;
  toggleStop: (stopId: string) => void;
  campaignId: string;
}

export default function NeighborhoodSection({
  name,
  stops,
  selectedStopIds,
  toggleStop,
  campaignId,
}: NeighborhoodSectionProps) {
  const [expanded, setExpanded] = useState(true);

  return (
    <div className="rounded border border-gray-200">
      <div
        className={
          "bg-gray-100 p-2 flex justify-between" +
          (expanded ? " rounded-t-sm" : " rounded")
        }
        onClick={() => setExpanded(!expanded)}
      >
        <div className="flex gap-2 items-center">
          <div className="rounded-full bg-blue-100 p-1">
            <BuildingOffice2Icon className="h-5 w-5 text-blue-600" />
          </div>
          <h2 className="text-lg text-gray-600">
            {name} ({stops.length} stop)
          </h2>
        </div>
        {expanded ? (
          <ChevronDownIcon className="h-5 w-5" />
        ) : (
          <ChevronUpIcon className="h-5 w-5" />
        )}
      </div>
      <div
        className={expanded ? "rounded-b-sm p-2" : "overflow-hidden max-h-0"}
      >
        <div className="space-y-2">
          {stops.map((stop) => (
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
    </div>
  );
}
