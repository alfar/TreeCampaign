import { BuildingOffice2Icon } from "@heroicons/react/24/outline";
import type { Stop } from "../../shared/api/models/stop";
import Section from "../../shared/components/Section";
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
  return (
    <Section
      icon={<BuildingOffice2Icon className="h-5 w-5 text-blue-600" />}
      title={`${name} (${stops.length} stop)`}
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
    </Section>
  );
}
