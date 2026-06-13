import { useEffect, useState } from "react";
import { createNeighborhood, createStreetSection, getNeighborhoods } from "../../shared/api/client";
import type { Neighborhood } from "../../shared/api/models/neighborhood";
import type { Order } from "../../shared/api/models/order";

interface CreateStreetSectionFormProps {
  order: Pick<Order, "message" | "houseNumber"> & { streetId: string };
  territoryId: string;
  onSectionCreated: () => void;
}

export default function CreateStreetSectionForm({ order, territoryId, onSectionCreated }: CreateStreetSectionFormProps) {
  const [neighborhoods, setNeighborhoods] = useState<Neighborhood[]>([]);
  const [neighborhoodInput, setNeighborhoodInput] = useState("");
  const [selectedNeighborhood, setSelectedNeighborhood] = useState<Neighborhood | null>(null);
  const [showSuggestions, setShowSuggestions] = useState(false);
  const [isAdding, setIsAdding] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getNeighborhoods(territoryId).then(setNeighborhoods);
  }, [territoryId]);

  const filteredNeighborhoods = neighborhoods.filter((n) =>
    n.name.toLowerCase().includes(neighborhoodInput.toLowerCase())
  );
  const noMatch = neighborhoodInput.trim().length > 0 && filteredNeighborhoods.length === 0;
  const canSubmit = selectedNeighborhood !== null && !isSubmitting;

  const selectNeighborhood = (n: Neighborhood) => {
    setSelectedNeighborhood(n);
    setNeighborhoodInput(n.name);
    setShowSuggestions(false);
    setError(null);
  };

  const handleInputChange = (value: string) => {
    setNeighborhoodInput(value);
    setSelectedNeighborhood(null);
    setShowSuggestions(true);
  };

  const handleAddNeighborhood = async () => {
    setIsAdding(true);
    try {
      const created = await createNeighborhood(territoryId, neighborhoodInput.trim());
      setNeighborhoods((prev) => [...prev, created]);
      selectNeighborhood(created);
    } finally {
      setIsAdding(false);
    }
  };

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (!selectedNeighborhood) return;

    setIsSubmitting(true);
    setError(null);
    try {
      const res = await createStreetSection(territoryId, selectedNeighborhood.id, order.streetId);
      if (res.ok) {
        onSectionCreated();
      } else {
        setError("Noget gik galt. Prøv igen.");
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4">
      <div>
        <p className="text-xs text-gray-500 mb-1">Original besked</p>
        <p className="text-sm text-gray-800 bg-gray-50 p-3 rounded border">{order.message}</p>
      </div>

      {order.houseNumber && (
        <div>
          <p className="text-xs text-gray-500 mb-1">Husnummer</p>
          <p className="text-sm font-medium text-gray-800">{order.houseNumber}</p>
        </div>
      )}

      <div className="relative">
        <label className="text-sm font-medium text-gray-700 block mb-1">Kvarter</label>
        <input
          type="text"
          value={neighborhoodInput}
          onChange={(e) => handleInputChange(e.target.value)}
          onFocus={() => setShowSuggestions(true)}
          onBlur={() => setTimeout(() => setShowSuggestions(false), 150)}
          className="w-full border rounded px-3 py-2 text-sm"
          placeholder="Søg efter kvarter…"
          autoComplete="off"
        />
        {showSuggestions && filteredNeighborhoods.length > 0 && (
          <ul className="absolute z-10 w-full bg-white border rounded shadow-md mt-1 max-h-48 overflow-y-auto">
            {filteredNeighborhoods.map((n) => (
              <li
                key={n.id}
                onMouseDown={() => selectNeighborhood(n)}
                className="px-3 py-2 text-sm hover:bg-gray-100 cursor-pointer"
              >
                {n.name}
              </li>
            ))}
          </ul>
        )}
        {noMatch && (
          <button
            type="button"
            onClick={handleAddNeighborhood}
            disabled={isAdding}
            className="mt-2 text-sm text-blue-600 hover:underline disabled:opacity-50"
          >
            {isAdding ? "Tilføjer…" : `+ Tilføj "${neighborhoodInput.trim()}" som nyt kvarter`}
          </button>
        )}
      </div>

      {error && <p className="text-sm text-red-600">{error}</p>}

      <button
        type="submit"
        disabled={!canSubmit}
        className="self-start bg-blue-600 text-white py-2 px-5 rounded disabled:opacity-40"
      >
        {isSubmitting ? "Opretter sektion…" : "Opret vejstrækning"}
      </button>
    </form>
  );
}
