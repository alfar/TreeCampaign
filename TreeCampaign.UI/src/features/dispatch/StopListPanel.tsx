import { MagnifyingGlassIcon } from "@heroicons/react/24/outline";
import type { Neighborhood } from "../../shared/api/models/neighborhood";
import type { Stop, StopType } from "../../shared/api/models/stop";
import NeighborhoodSection from "./NeighborhoodSection";
import StopCard from "./StopCard";
import StopTypeFilter from "./StopTypeFilter";

interface StopListPanelProps {
  campaignId: string;
  selectedStopTypes: Set<StopType>;
  setSelectedStopTypes: (value: Set<StopType>) => void;
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
  selectedStopTypes,
  setSelectedStopTypes,
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
          {filter.length > 0
            ? `Stop, der starter med '${filter}' `
            : "Stop "}
          - {sortedStops.length} stop
        </h2>
      </div>
      <div className="flex gap-4 flex-wrap">
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
        <StopTypeFilter
          selectedStopTypes={selectedStopTypes}
          setSelectedStopTypes={setSelectedStopTypes}
        />
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
