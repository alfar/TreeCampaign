import { MagnifyingGlassIcon } from "@heroicons/react/24/outline";
import type { Neighborhood } from "../../shared/api/models/neighborhood";
import type { Stop, StopType } from "../../shared/api/models/stop";
import NeighborhoodSection from "./NeighborhoodSection";
import StopCard from "./StopCard";
import type { TrailerSize } from "../../shared/api/models/team";
import StopMaxTrailerSizeFilter from "./StopMaxTrailerSizeFilter";
import StopTypeFilter from "./StopTypeFilter";
import type { NeighborhoodStopCounts } from "./useDispatchDerivedState";

interface StopListPanelProps {
  campaignId: string;
  selectedStopTypes: Set<StopType>;
  setSelectedStopTypes: (value: Set<StopType>) => void;
  selectedMaxTrailerSizes: Set<TrailerSize>;
  setSelectedMaxTrailerSizes: (value: Set<TrailerSize>) => void;
  filter: string;
  setFilter: (value: string) => void;
  sortedStops: Stop[];
  stopsByNeighborhood: {
    neighborhood: Neighborhood;
    stops: Stop[];
    counts: NeighborhoodStopCounts;
  }[];
  ungroupedStops: Stop[];
  getTeamName: (teamId: string | undefined) => string | undefined;
  selectedStopIds: Set<string>;
  /** Trees across all selected stops, including any hidden by filters. */
  selectedTreeCount: number;
  toggleStop: (stopId: string) => void;
  onSelectStops: (stopIds: string[]) => void;
}

export default function StopListPanel({
  campaignId,
  selectedStopTypes,
  setSelectedStopTypes,
  selectedMaxTrailerSizes,
  setSelectedMaxTrailerSizes,
  filter,
  setFilter,
  sortedStops,
  stopsByNeighborhood,
  ungroupedStops,
  getTeamName,
  selectedStopIds,
  selectedTreeCount,
  toggleStop,
  onSelectStops,
}: StopListPanelProps) {
  return (
    <div className="w-8/12 flex flex-col gap-2 border border-gray-300 rounded-lg p-4">
      <div className="flex items-center align-center h-11">
        <h2 className="text-base">
          {filter.length > 0
            ? `Stop, der starter med '${filter}' `
            : "Stop "}
          - {sortedStops.length} stop
        </h2>
      </div>
      <div className="flex gap-4 flex-wrap items-center">
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
        <StopMaxTrailerSizeFilter
          selectedSizes={selectedMaxTrailerSizes}
          setSelectedSizes={setSelectedMaxTrailerSizes}
        />
      </div>
      {stopsByNeighborhood.map(({ neighborhood, stops: nStops, counts }) => (
        <NeighborhoodSection
          key={neighborhood.id}
          name={neighborhood.name}
          stops={nStops}
          counts={counts}
          getTeamName={getTeamName}
          campaignId={campaignId}
          selectedStopIds={selectedStopIds}
          toggleStop={toggleStop}
          onSelectStops={onSelectStops}
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
      {selectedStopIds.size > 0 && (
        <div className="sticky bottom-0 z-10 -mx-4 -mb-4 flex items-center justify-between gap-4 rounded-t-2xl bg-white px-4 py-3 shadow-[0_-4px_16px_rgba(0,0,0,0.2)]">
          <span className="text-lg text-gray-800">
            <span className="font-bold">{selectedTreeCount}</span> træer valgt
            <span className="ml-2 text-sm text-gray-500">
              ({selectedStopIds.size} stop)
            </span>
          </span>
          <button
            type="button"
            onClick={() => onSelectStops([])}
            className="ml-auto h-12 min-w-28 rounded-lg bg-blue-600 px-5 text-base font-medium text-white active:bg-blue-700"
          >
            Ryd valg
          </button>
        </div>
      )}
    </div>
  );
}
