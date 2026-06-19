import { useState } from "react";
import { createTerritory } from "../../shared/api/client";
import type { Territory } from "../../shared/api/models/territory";

interface CreateTerritoryFormProps {
  onCreated: (territory: Territory) => void;
}

export default function CreateTerritoryForm({ onCreated }: CreateTerritoryFormProps) {
  const [name, setName] = useState("");
  const [defaultZipCode, setDefaultZipCode] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const canSubmit = name.trim().length > 0 && defaultZipCode.trim().length === 4 && !isSubmitting;

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (!canSubmit) return;

    setIsSubmitting(true);
    setError(null);
    try {
      const territory = await createTerritory(name.trim(), defaultZipCode.trim());
      setName("");
      setDefaultZipCode("");
      onCreated(territory);
    } catch {
      setError("Noget gik galt. Prøv igen.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4 p-4 border rounded bg-gray-50">
      <h2 className="text-base font-semibold">Nyt territorium</h2>
      <div className="flex gap-3">
        <div className="flex-1">
          <label className="text-sm font-medium text-gray-700 block mb-1">Navn</label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
            placeholder="Silkeborg Nord"
          />
        </div>
        <div className="w-28">
          <label className="text-sm font-medium text-gray-700 block mb-1">Postnr.</label>
          <input
            type="text"
            inputMode="numeric"
            maxLength={4}
            value={defaultZipCode}
            onChange={(e) => setDefaultZipCode(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
            placeholder="8600"
          />
        </div>
      </div>
      {error && <p className="text-sm text-red-600">{error}</p>}
      <button
        type="submit"
        disabled={!canSubmit}
        className="self-start bg-blue-600 text-white py-2 px-5 rounded disabled:opacity-40"
      >
        {isSubmitting ? "Opretter…" : "Opret territorium"}
      </button>
    </form>
  );
}
