import { ChevronDownIcon, ChevronUpIcon } from "@heroicons/react/24/outline";
import { useState } from "react";
import { stopTypeLabels, type StopType } from "../../shared/api/models/stop";

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
  const [expanded, setExpanded] = useState(false);

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
    <div className="flex flex-col">
      <button
        type="button"
        onClick={() => setExpanded((v) => !v)}
        className="flex items-center gap-1 text-sm text-gray-600 hover:text-gray-900"
      >
        {expanded ? (
          <ChevronUpIcon className="h-4 w-4" />
        ) : (
          <ChevronDownIcon className="h-4 w-4" />
        )}
        {selectedStopTypesSummary}
      </button>
      {expanded && (
        <div className="flex gap-3 flex-wrap items-center mt-2">
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
        </div>
      )}
    </div>
  );
}
