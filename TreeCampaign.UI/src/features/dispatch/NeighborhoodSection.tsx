import { BuildingOffice2Icon } from "@heroicons/react/24/outline";
import type { Stop } from "../../shared/api/models/stop";
import Section from "../../shared/components/Section";
import StopCard from "./StopCard";
import ProgressBar from "../../components/ProgressBar";
import type { NeighborhoodStopCounts } from "./useDispatchDerivedState";

interface NeighborhoodSectionProps {
  name: string;
  stops: Stop[];
  counts: NeighborhoodStopCounts;
  getTeamName: (teamId: string | undefined) => string | undefined;
  selectedStopIds: Set<string>;
  toggleStop: (stopId: string) => void;
  campaignId: string;
}

export default function NeighborhoodSection({
  name,
  stops,
  counts,
  getTeamName,
  selectedStopIds,
  toggleStop,
  campaignId,
}: NeighborhoodSectionProps) {
  const progressParts = [
            { title: "Opsamlet", amount: counts.collected, color: "#16a34a" },
            { title: "Fejlet", amount: counts.unresolved, color: "#dc2626" },
            { title: "Tildelt", amount: counts.pending, color: "#2563eb" },
            { title: "Mangler", amount: counts.unassigned, color: "#ffffff" }
          ];

  return (
    <Section
      icon={<BuildingOffice2Icon className="h-5 w-5 text-blue-600" />}
      titleNode={
        <div className="flex gap-2 justify-between grow">
          <h2 className="text-nowrap">{name}</h2>
          <div className="w-1/3 mr-5">
            <ProgressBar
              parts={progressParts}
              total={counts.total}
              textSize="text-xs"
            />
          </div>
        </div>
      }
    >
      <div className="space-y-2">
        {stops.map((stop) => (
          <StopCard
            key={stop.id}
            campaignId={campaignId}
            stop={stop}
            teamName={getTeamName(stop.assignedTeamId)}
            assignMode={true}
            selected={selectedStopIds.has(stop.id)}
            onToggleSelect={toggleStop}
          />
        ))}
      </div>
    </Section>
  );
}
