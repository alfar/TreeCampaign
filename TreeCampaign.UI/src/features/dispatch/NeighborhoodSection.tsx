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
  onSelectStops: (stopIds: string[]) => void;
  campaignId: string;
}

/** 0 clears the selection. */
const treeCountShortcuts = [0, 8, 12, 16];

/**
 * The first unassigned stops, in list order, whose tree amounts add up to at
 * most `maxTrees`. Stops at the first stop that would overshoot, so the
 * selection stays one contiguous run along the route.
 */
function firstStopsUpToTrees(stops: Stop[], maxTrees: number): string[] {
  const ids: string[] = [];
  let trees = 0;
  for (const stop of stops) {
    if (stop.stopType !== "Unassigned") continue;
    if (trees + stop.amount > maxTrees) break;
    trees += stop.amount;
    ids.push(stop.id);
  }
  return ids;
}

export default function NeighborhoodSection({
  name,
  stops,
  counts,
  getTeamName,
  selectedStopIds,
  toggleStop,
  onSelectStops,
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
        <div className="flex gap-2 justify-between items-center grow">
          <h2 className="text-nowrap">{name}</h2>
          <div
            className="ml-auto flex items-center gap-2"
            onClick={(e) => e.stopPropagation()}
          >
            {treeCountShortcuts.map((n) => (
              <button
                key={n}
                type="button"
                title={
                  n === 0
                    ? "Ryd valgte stop"
                    : `Vælg de første stop op til ${n} træer`
                }
                onClick={() => onSelectStops(firstStopsUpToTrees(stops, n))}
                className="min-w-14 h-11 rounded-md border border-gray-300 bg-white px-3 text-base font-medium text-gray-700 hover:bg-blue-50 active:bg-blue-100"
              >
                {n}
              </button>
            ))}
          </div>
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
