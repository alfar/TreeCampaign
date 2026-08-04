import { useState } from "react";
import { updateStreetSection } from "../../shared/api/client";
import type { Neighborhood } from "../../shared/api/models/neighborhood";
import type { StreetSection } from "../../shared/api/models/streetSection";
import { trailerSizeLabels, type TrailerSize } from "../../shared/api/models/team";

interface EditStreetSectionFormProps {
  territoryId: string;
  neighborhoodId: string;
  section: StreetSection;
  onSaved: (neighborhood: Neighborhood) => void;
  onCancel: () => void;
}

export default function EditStreetSectionForm({
  territoryId,
  neighborhoodId,
  section,
  onSaved,
  onCancel,
}: EditStreetSectionFormProps) {
  const [evenFromHouseNumber, setEvenFromHouseNumber] = useState(section.evenStartHouseNumber ?? "");
  const [evenToHouseNumber, setEvenToHouseNumber] = useState(section.evenEndHouseNumber ?? "");
  const [oddFromHouseNumber, setOddFromHouseNumber] = useState(section.oddStartHouseNumber ?? "");
  const [oddToHouseNumber, setOddToHouseNumber] = useState(section.oddEndHouseNumber ?? "");
  const [sortOrder, setSortOrder] = useState(String(section.sortOrder));
  const [direction, setDirection] = useState<0 | 1>(section.direction === 1 ? 1 : 0);
  const [maxTrailerSize, setMaxTrailerSize] = useState<TrailerSize>(section.maxTrailerSize);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const canSubmit = sortOrder.trim().length > 0 && !isSubmitting;

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (!canSubmit) return;

    setIsSubmitting(true);
    setError(null);
    try {
      const res = await updateStreetSection(
        territoryId,
        neighborhoodId,
        section.id,
        Number(sortOrder),
        evenFromHouseNumber.trim() || null,
        evenToHouseNumber.trim() || null,
        oddFromHouseNumber.trim() || null,
        oddToHouseNumber.trim() || null,
        direction,
        maxTrailerSize,
      );
      if (res.ok) {
        const neighborhood: Neighborhood = await res.json();
        onSaved(neighborhood);
      } else {
        setError("Noget gik galt. Prøv igen.");
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-3 p-3 border rounded bg-gray-50">
      <div className="flex gap-3">
        <div className="flex-1">
          <label className="text-sm font-medium text-gray-700 block mb-1">Lige husnumre, fra (valgfri)</label>
          <input
            type="text"
            value={evenFromHouseNumber}
            onChange={(e) => setEvenFromHouseNumber(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
            placeholder="2"
          />
        </div>
        <div className="flex-1">
          <label className="text-sm font-medium text-gray-700 block mb-1">Lige husnumre, til (valgfri)</label>
          <input
            type="text"
            value={evenToHouseNumber}
            onChange={(e) => setEvenToHouseNumber(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
            placeholder="98"
          />
        </div>
      </div>

      <div className="flex gap-3">
        <div className="flex-1">
          <label className="text-sm font-medium text-gray-700 block mb-1">Ulige husnumre, fra (valgfri)</label>
          <input
            type="text"
            value={oddFromHouseNumber}
            onChange={(e) => setOddFromHouseNumber(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
            placeholder="1"
          />
        </div>
        <div className="flex-1">
          <label className="text-sm font-medium text-gray-700 block mb-1">Ulige husnumre, til (valgfri)</label>
          <input
            type="text"
            value={oddToHouseNumber}
            onChange={(e) => setOddToHouseNumber(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
            placeholder="99"
          />
        </div>
        <div className="w-28">
          <label className="text-sm font-medium text-gray-700 block mb-1">Rækkefølge</label>
          <input
            type="number"
            value={sortOrder}
            onChange={(e) => setSortOrder(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
          />
        </div>
      </div>

      <div>
        <label className="text-sm font-medium text-gray-700 block mb-1">Retning</label>
        <div className="flex gap-2">
          <button
            type="button"
            onClick={() => setDirection(0)}
            className={`flex-1 py-2 rounded text-sm border ${direction === 0 ? "bg-blue-600 text-white border-blue-600" : "border-gray-300"}`}
          >
            Stigende
          </button>
          <button
            type="button"
            onClick={() => setDirection(1)}
            className={`flex-1 py-2 rounded text-sm border ${direction === 1 ? "bg-blue-600 text-white border-blue-600" : "border-gray-300"}`}
          >
            Faldende
          </button>
        </div>
      </div>

      <div>
        <label className="text-sm font-medium text-gray-700 block mb-1">Maks. trailerstørrelse</label>
        <div className="flex gap-2">
          {(Object.keys(trailerSizeLabels) as TrailerSize[]).map((size) => (
            <button
              key={size}
              type="button"
              onClick={() => setMaxTrailerSize(size)}
              className={`flex-1 py-2 rounded text-sm border ${maxTrailerSize === size ? "bg-blue-600 text-white border-blue-600" : "border-gray-300"}`}
            >
              {trailerSizeLabels[size]}
            </button>
          ))}
        </div>
      </div>

      {error && <p className="text-sm text-red-600">{error}</p>}

      <div className="flex gap-2">
        <button
          type="submit"
          disabled={!canSubmit}
          className="bg-blue-600 text-white py-2 px-5 rounded text-sm disabled:opacity-40"
        >
          {isSubmitting ? "Gemmer…" : "Gem"}
        </button>
        <button type="button" onClick={onCancel} className="py-2 px-5 rounded border text-sm">
          Annuller
        </button>
      </div>
    </form>
  );
}
