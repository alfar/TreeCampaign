import { BuildingOffice2Icon } from "@heroicons/react/24/outline";
import type { Stop } from "../../shared/api/models/stop";
import Section from "../../shared/components/Section";
import StopCard from "./StopCard";
import ProgressBar from "../../components/ProgressBar";

interface NeighborhoodSectionProps {
  name: string;
  stops: Stop[];
  getTeamName: (teamId: string | undefined) => string | undefined;
  selectedStopIds: Set<string>;
  toggleStop: (stopId: string) => void;
  campaignId: string;
}

const STOP_COLORS: Record<string, string> = {
  Unassigned: "#2563eb",
  Assigned: "#2563eb",
  Collected: "#16a34a",
  Delivered: "#16a34a",
  Unresolved: "#dc2626",
};

export default function NeighborhoodSection({
  name,
  stops,
  getTeamName,
  selectedStopIds,
  toggleStop,
  campaignId,
}: NeighborhoodSectionProps) {

  const counts = stops.reduce(
    (acc, stop) => {
      acc.total += stop.amount;
      switch (stop.stopType) {
        case "Unassigned":
          acc.unassigned += stop.amount;
          break;
        case "Assigned":
          acc.pending += stop.amount;
          break;
        case "Unresolved":
        case "Abandoned":
          acc.unresolved += stop.amount;
          break;
        case "Collected":
        case "Delivered":
          acc.collected += stop.amount;
          break;
      }
      return acc;
    },
    { unassigned: 0,pending: 0, unresolved: 0, collected: 0, total: 0 },
  );

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
              total={stops.length}
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
