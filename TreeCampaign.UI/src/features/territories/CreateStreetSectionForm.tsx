import { useEffect, useState } from "react";
import { createStreet, createStreetSection, getStreetsByZipCode } from "../../shared/api/client";
import { searchStreets, type StreetCandidate } from "../../shared/api/adressevaelger";
import type { Street } from "../../shared/api/models/street";
import type { Neighborhood } from "../../shared/api/models/neighborhood";
import { trailerSizeLabels, type TrailerSize } from "../../shared/api/models/team";

interface CreateStreetSectionFormProps {
  territoryId: string;
  neighborhoodId: string;
  defaultZipCode: string;
  onCreated: (neighborhood: Neighborhood) => void;
  onCancel: () => void;
}

export default function CreateStreetSectionForm({
  territoryId,
  neighborhoodId,
  defaultZipCode,
  onCreated,
  onCancel,
}: CreateStreetSectionFormProps) {
  const [zipCode, setZipCode] = useState(defaultZipCode);
  const [streetSearch, setStreetSearch] = useState("");
  const [selectedStreetName, setSelectedStreetName] = useState<string | null>(null);
  const [streetCandidates, setStreetCandidates] = useState<StreetCandidate[]>([]);
  const [resolvedStreet, setResolvedStreet] = useState<Street | null>(null);
  const [isCreatingStreet, setIsCreatingStreet] = useState(false);

  const [evenFromHouseNumber, setEvenFromHouseNumber] = useState("");
  const [evenToHouseNumber, setEvenToHouseNumber] = useState("");
  const [oddFromHouseNumber, setOddFromHouseNumber] = useState("");
  const [oddToHouseNumber, setOddToHouseNumber] = useState("");
  const [sortOrder, setSortOrder] = useState("0");
  const [direction, setDirection] = useState<0 | 1>(0);
  const [maxTrailerSize, setMaxTrailerSize] = useState<TrailerSize>("Boogie");

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const streetName = selectedStreetName ?? streetSearch;

  useEffect(() => {
    if (selectedStreetName || zipCode.length !== 4 || streetSearch.trim().length < 2) {
      setStreetCandidates([]);
      return;
    }

    let cancelled = false;
    const timeout = setTimeout(() => {
      searchStreets(streetSearch.trim(), zipCode).then((candidates) => {
        if (!cancelled) setStreetCandidates(candidates);
      });
    }, 300);

    return () => {
      cancelled = true;
      clearTimeout(timeout);
    };
  }, [streetSearch, zipCode, selectedStreetName]);

  useEffect(() => {
    if (!selectedStreetName || zipCode.length !== 4) {
      setResolvedStreet(null);
      return;
    }

    let cancelled = false;
    getStreetsByZipCode(zipCode).then((streets) => {
      if (cancelled) return;
      const match = streets.find((s) => s.name.toLowerCase() === selectedStreetName.toLowerCase());
      setResolvedStreet(match ?? null);
    });

    return () => {
      cancelled = true;
    };
  }, [selectedStreetName, zipCode]);

  const selectStreet = (name: string) => {
    setSelectedStreetName(name);
    setStreetCandidates([]);
  };

  const handleCreateStreet = async () => {
    if (!selectedStreetName || zipCode.length !== 4) return;
    setIsCreatingStreet(true);
    try {
      const street = await createStreet(selectedStreetName, zipCode);
      setResolvedStreet(street);
    } finally {
      setIsCreatingStreet(false);
    }
  };

  const canSubmit = resolvedStreet !== null && sortOrder.trim().length > 0 && !isSubmitting;

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (!resolvedStreet || !canSubmit) return;

    setIsSubmitting(true);
    setError(null);
    try {
      const res = await createStreetSection(
        territoryId,
        neighborhoodId,
        resolvedStreet.id,
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
        onCreated(neighborhood);
      } else {
        setError("Noget gik galt. Prøv igen.");
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-3 p-4 border rounded bg-gray-50">
      <h2 className="text-base font-semibold">Ny vejstrækning</h2>

      <div className="flex gap-3">
        <div className="flex-1 relative">
          <label className="text-sm font-medium text-gray-700 block mb-1">Gadenavn</label>
          <input
            type="text"
            placeholder="Søg gade…"
            value={streetName}
            onChange={(e) => {
              setStreetSearch(e.target.value);
              setSelectedStreetName(null);
            }}
            className="w-full border rounded px-3 py-2 text-sm"
            autoComplete="off"
          />
          {streetCandidates.length > 0 && !selectedStreetName && (
            <ul className="absolute z-10 w-full bg-white border rounded mt-1 max-h-48 overflow-y-auto shadow">
              {streetCandidates.map((c) => (
                <li
                  key={c.id}
                  className="px-3 py-2 text-sm cursor-pointer hover:bg-blue-50"
                  onClick={() => selectStreet(c.vejnavn)}
                >
                  {c.vejnavn}
                </li>
              ))}
            </ul>
          )}
          {selectedStreetName && resolvedStreet === null && (
            <button
              type="button"
              onClick={handleCreateStreet}
              disabled={isCreatingStreet}
              className="mt-2 text-sm text-blue-600 hover:underline disabled:opacity-50"
            >
              {isCreatingStreet ? "Tilføjer…" : `+ Tilføj "${selectedStreetName}" som ny gade`}
            </button>
          )}
          {resolvedStreet && (
            <p className="text-xs text-green-600 mt-1">Gade fundet</p>
          )}
        </div>

        <div className="w-24">
          <label className="text-sm font-medium text-gray-700 block mb-1">Postnr.</label>
          <input
            type="text"
            inputMode="numeric"
            maxLength={4}
            value={zipCode}
            onChange={(e) => {
              setZipCode(e.target.value);
              setSelectedStreetName(null);
              setStreetSearch("");
            }}
            className="w-full border rounded px-3 py-2 text-sm"
          />
        </div>
      </div>

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
          {isSubmitting ? "Opretter…" : "Opret vejstrækning"}
        </button>
        <button type="button" onClick={onCancel} className="py-2 px-5 rounded border text-sm">
          Annuller
        </button>
      </div>
    </form>
  );
}
