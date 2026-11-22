import { stopTypeLabels, type StopType } from "../../shared/api/models/stop";
import FilterPopover from "./FilterPopover";

const filterableStopTypes: StopType[] = [
  "Unassigned",
  "Assigned",
  "Unresolved",
  "Abandoned",
  "Collected",
  "Delivered",
];

interface StopTypeFilterProps {
  selectedStopTypes: Set<StopType>;
  setSelectedStopTypes: (value: Set<StopType>) => void;
}

export default function StopTypeFilter({
  selectedStopTypes,
  setSelectedStopTypes,
}: StopTypeFilterProps) {
  const toggleStopType = (stopType: StopType) => {
    const next = new Set(selectedStopTypes);
    if (next.has(stopType)) {
      next.delete(stopType);
    } else {
      next.add(stopType);
    }
    setSelectedStopTypes(next);
  };

  const selectedStopTypesSummary =
    filterableStopTypes
      .filter((stopType) => selectedStopTypes.has(stopType))
      .map((stopType) => stopTypeLabels[stopType])
      .join(", ") || "Ingen valgt";

  return (
    <FilterPopover label={selectedStopTypesSummary}>
      {filterableStopTypes.map((stopType) => (
        <label
          key={stopType}
          htmlFor={`stopType-${stopType}`}
          className="flex items-center text-sm"
        >
          <input
            type="checkbox"
            id={`stopType-${stopType}`}
            className="w-4 h-4 mr-2"
            checked={selectedStopTypes.has(stopType)}
            onChange={() => toggleStopType(stopType)}
          />
          {stopTypeLabels[stopType]}
        </label>
      ))}
    </FilterPopover>
  );
}
