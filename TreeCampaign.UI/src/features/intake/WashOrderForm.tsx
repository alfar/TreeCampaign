import { useEffect, useState } from "react";
import { createStreet, getStreetsByZipCode, washOrder } from "../../shared/api/client";
import type { Order } from "../../shared/api/models/order";
import type { Street } from "../../shared/api/models/street";

interface WashOrderFormProps {
  order: Pick<Order, "id" | "message">;
  campaignId: string;
  defaultZipCode: string;
  onStreetAdded?: () => void;
  onWashed?: () => void;
}

export default function WashOrderForm({ order, campaignId, defaultZipCode, onStreetAdded, onWashed }: WashOrderFormProps) {
  const [zipCode, setZipCode] = useState(defaultZipCode);
  const [streets, setStreets] = useState<Street[]>([]);
  const [streetInput, setStreetInput] = useState("");
  const [selectedStreet, setSelectedStreet] = useState<Street | null>(null);
  const [showSuggestions, setShowSuggestions] = useState(false);
  const [houseNumber, setHouseNumber] = useState("");
  const [isAdding, setIsAdding] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [washError, setWashError] = useState<string | null>(null);

  useEffect(() => {
    if (zipCode.length === 4) {
      getStreetsByZipCode(zipCode).then(setStreets);
    } else {
      setStreets([]);
    }
    setSelectedStreet(null);
    setStreetInput("");
  }, [zipCode]);

  const filteredStreets = streets.filter((s) =>
    s.name.toLowerCase().includes(streetInput.toLowerCase())
  );
  const noMatch = streetInput.trim().length > 0 && filteredStreets.length === 0;
  const canSubmit = selectedStreet !== null && houseNumber.trim().length > 0 && !isSubmitting;

  const selectStreet = (street: Street) => {
    setSelectedStreet(street);
    setStreetInput(street.name);
    setShowSuggestions(false);
    setWashError(null);
  };

  const handleAddStreet = async () => {
    setIsAdding(true);
    try {
      await createStreet(streetInput.trim(), zipCode);
      getStreetsByZipCode(zipCode).then(setStreets);
      onStreetAdded?.();
    } finally {
      setIsAdding(false);
    } 
  };

  const handleStreetInputChange = (value: string) => {
    setStreetInput(value);
    setSelectedStreet(null);
    setShowSuggestions(true);
  };

  const handleHouseNumberChange = (value: string) => {
    setHouseNumber(value);
    setWashError(null);
  };

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (!selectedStreet || !houseNumber.trim()) return;

    setIsSubmitting(true);
    setWashError(null);
    try {
      const res = await washOrder(campaignId, order.id, {
        streetId: selectedStreet.id,
        houseNumber: houseNumber.trim(),
      });
      if (res.ok) {
        onWashed?.();
      } else if (res.status === 422) {
        setWashError("Husnummeret matcher ingen sektion på denne gade.");
      } else {
        setWashError("Noget gik galt. Prøv igen.");
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

      <div>
        <label className="text-sm font-medium text-gray-700 block mb-1">Postnummer</label>
        <input
          type="text"
          inputMode="numeric"
          maxLength={4}
          value={zipCode}
          onChange={(e) => setZipCode(e.target.value)}
          className="w-28 border rounded px-3 py-2 text-sm"
          placeholder="8600"
        />
      </div>

      <div className="relative">
        <label className="text-sm font-medium text-gray-700 block mb-1">Gade</label>
        <input
          type="text"
          value={streetInput}
          onChange={(e) => handleStreetInputChange(e.target.value)}
          onFocus={() => setShowSuggestions(true)}
          onBlur={() => setTimeout(() => setShowSuggestions(false), 150)}
          className="w-full border rounded px-3 py-2 text-sm"
          placeholder="Søg efter gade…"
          autoComplete="off"
        />
        {showSuggestions && filteredStreets.length > 0 && (
          <ul className="absolute z-10 w-full bg-white border rounded shadow-md mt-1 max-h-48 overflow-y-auto">
            {filteredStreets.map((street) => (
              <li
                key={street.id}
                onMouseDown={() => selectStreet(street)}
                className="px-3 py-2 text-sm hover:bg-gray-100 cursor-pointer"
              >
                {street.name}
              </li>
            ))}
          </ul>
        )}
        {noMatch && (
          <button
            type="button"
            onClick={handleAddStreet}
            disabled={isAdding}
            className="mt-2 text-sm text-blue-600 hover:underline disabled:opacity-50"
          >
            {isAdding ? "Tilføjer…" : `+ Tilføj "${streetInput.trim()}" som ny gade`}
          </button>
        )}
      </div>

      <div>
        <label className="text-sm font-medium text-gray-700 block mb-1">Husnummer</label>
        <input
          type="text"
          value={houseNumber}
          onChange={(e) => handleHouseNumberChange(e.target.value)}
          className="w-28 border rounded px-3 py-2 text-sm"
          placeholder="42B"
        />
      </div>

      {washError && (
        <p className="text-sm text-red-600">{washError}</p>
      )}

      <button
        type="submit"
        disabled={!canSubmit}
        className="self-start bg-blue-600 text-white py-2 px-5 rounded disabled:opacity-40"
      >
        {isSubmitting ? "Gemmer…" : "Gem adresse"}
      </button>
    </form>
  );
}
