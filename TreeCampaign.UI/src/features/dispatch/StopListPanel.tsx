import { MagnifyingGlassIcon } from "@heroicons/react/24/outline";
import type { Neighborhood } from "../../shared/api/models/neighborhood";
import type { Stop } from "../../shared/api/models/stop";
import NeighborhoodSection from "./NeighborhoodSection";
import StopCard from "./StopCard";

interface StopListPanelProps {
  campaignId: string;
  onlyUnassigned: boolean;
  setOnlyUnassigned: (value: boolean) => void;
  filter: string;
  setFilter: (value: string) => void;
  sortedStops: Stop[];
  stopsByNeighborhood: { neighborhood: Neighborhood; stops: Stop[] }[];
  ungroupedStops: Stop[];
  getTeamName: (teamId: string | undefined) => string | undefined;
  selectedStopIds: Set<string>;
  toggleStop: (stopId: string) => void;
}

export default function StopListPanel({
  campaignId,
  onlyUnassigned,
  setOnlyUnassigned,
  filter,
  setFilter,
  sortedStops,
  stopsByNeighborhood,
  ungroupedStops,
  getTeamName,
  selectedStopIds,
  toggleStop,
}: StopListPanelProps) {
  return (
    <div className="w-8/12 flex flex-col gap-2 border border-gray-300 rounded-lg p-4">
      <div className="flex items-center h-11">
        <h2 className="text-base font-semibold">
          {(onlyUnassigned ? "Frie" : "Alle") +
            (filter.length > 0
              ? ` stop, der starter med '${filter}' `
              : " stop ")}
          - {sortedStops.length} stop
        </h2>
      </div>
      <div className="flex gap-2">
        <div className="flex border rounded-sm p-2 border-gray-200 w-6/12 items-center">
          <MagnifyingGlassIcon className="h-5 w-5 mr-2" />
          <input
            type="text"
            value={filter}
            className="w-full text-gray-500 focus:outline-0"
            placeholder="Søg efter stop"
            onChange={(e) => setFilter(e.target.value)}
          />
        </div>
        <label htmlFor="onlyUnassigned" className="flex items-center">
          <input
            type="checkbox"
            id="onlyUnassigned"
            className="w-4 h-4 mr-2"
            checked={onlyUnassigned}
            onChange={(e) => setOnlyUnassigned(e.target.checked)}
          />
          Vis kun frie stop
        </label>
      </div>
      {stopsByNeighborhood.map(({ neighborhood, stops: nStops }) => (
        <NeighborhoodSection
          key={neighborhood.id}
          name={neighborhood.name}
          stops={nStops}
          getTeamName={getTeamName}
          campaignId={campaignId}
          selectedStopIds={selectedStopIds}
          toggleStop={toggleStop}
        />
      ))}
      {ungroupedStops.map((stop) => (
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
      {stopsByNeighborhood.length === 0 && ungroupedStops.length === 0 && (
        <div className="w-full bg-gray-50 rounded-sm p-4 text-center text-sm text-gray-600">
          Ingen stop
        </div>
      )}
    </div>
  );
}
