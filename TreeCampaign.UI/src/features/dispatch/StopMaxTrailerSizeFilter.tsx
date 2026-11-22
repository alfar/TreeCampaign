import {
  trailerSizeLabels,
  trailerSizeOrder,
  type TrailerSize,
} from "../../shared/api/models/team";
import FilterPopover from "./FilterPopover";

/** All sizes a street section can declare as its max, including 'Runner'. */
export const allMaxTrailerSizes: TrailerSize[] = (
  Object.keys(trailerSizeOrder) as TrailerSize[]
).sort((a, b) => trailerSizeOrder[a] - trailerSizeOrder[b]);

interface StopMaxTrailerSizeFilterProps {
  selectedSizes: Set<TrailerSize>;
  setSelectedSizes: (value: Set<TrailerSize>) => void;
}

export default function StopMaxTrailerSizeFilter({
  selectedSizes,
  setSelectedSizes,
}: StopMaxTrailerSizeFilterProps) {
  const toggleSize = (size: TrailerSize) => {
    const next = new Set(selectedSizes);
    if (next.has(size)) {
      next.delete(size);
    } else {
      next.add(size);
    }
    setSelectedSizes(next);
  };

  const summary =
    selectedSizes.size === allMaxTrailerSizes.length
      ? "Alle trailerstørrelser"
      : allMaxTrailerSizes
          .filter((size) => selectedSizes.has(size))
          .map((size) => trailerSizeLabels[size])
          .join(", ") || "Ingen valgt";

  return (
    <FilterPopover label={`Max trailer: ${summary}`}>
      {allMaxTrailerSizes.map((size) => (
        <label
          key={size}
          htmlFor={`maxTrailerSize-${size}`}
          className="flex items-center text-sm"
        >
          <input
            type="checkbox"
            id={`maxTrailerSize-${size}`}
            className="w-4 h-4 mr-2"
            checked={selectedSizes.has(size)}
            onChange={() => toggleSize(size)}
          />
          {trailerSizeLabels[size]}
        </label>
      ))}
    </FilterPopover>
  );
}
