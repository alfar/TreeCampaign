import { useState } from "react";
import { createNeighborhood } from "../../shared/api/client";
import type { Neighborhood } from "../../shared/api/models/neighborhood";

interface CreateNeighborhoodFormProps {
  territoryId: string;
  onCreated: (neighborhood: Neighborhood) => void;
  onCancel: () => void;
}

export default function CreateNeighborhoodForm({ territoryId, onCreated, onCancel }: CreateNeighborhoodFormProps) {
  const [name, setName] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const canSubmit = name.trim().length > 0 && !isSubmitting;

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (!canSubmit) return;

    setIsSubmitting(true);
    setError(null);
    try {
      const neighborhood = await createNeighborhood(territoryId, name.trim());
      setName("");
      onCreated(neighborhood);
    } catch {
      setError("Noget gik galt. Prøv igen.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-3 p-4 border rounded bg-gray-50">
      <h2 className="text-base font-semibold">Nyt kvarter</h2>
      <div>
        <label className="text-sm font-medium text-gray-700 block mb-1">Navn</label>
        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          className="w-full border rounded px-3 py-2 text-sm"
          placeholder="Vestre Kvarter"
          autoFocus
        />
      </div>
      {error && <p className="text-sm text-red-600">{error}</p>}
      <div className="flex gap-2">
        <button
          type="submit"
          disabled={!canSubmit}
          className="bg-blue-600 text-white py-2 px-5 rounded text-sm disabled:opacity-40"
        >
          {isSubmitting ? "Opretter…" : "Opret kvarter"}
        </button>
        <button type="button" onClick={onCancel} className="py-2 px-5 rounded border text-sm">
          Annuller
        </button>
      </div>
    </form>
  );
}
