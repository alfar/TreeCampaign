import { useEffect, useState } from "react";
import { getTerritories, transferOrder } from "../../shared/api/client";
import type { Order } from "../../shared/api/models/order";
import type { Territory } from "../../shared/api/models/territory";

interface TransferOrderFormProps {
  order: Pick<Order, "id" | "message">;
  campaignId: string;
  currentTerritoryId: string | null;
  onTransferred: () => void;
}

export default function TransferOrderForm({ order, campaignId, currentTerritoryId, onTransferred }: TransferOrderFormProps) {
  const [territories, setTerritories] = useState<Territory[]>([]);
  const [selectedTerritoryId, setSelectedTerritoryId] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getTerritories().then(setTerritories);
  }, []);

  const options = territories.filter((t) => t.id !== currentTerritoryId);
  const canSubmit = selectedTerritoryId !== "" && !isSubmitting;

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (!selectedTerritoryId) return;

    setIsSubmitting(true);
    setError(null);
    try {
      const res = await transferOrder(campaignId, order.id, selectedTerritoryId);
      if (res.ok) {
        onTransferred();
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

      <div>
        <label className="text-sm font-medium text-gray-700 block mb-1">Område</label>
        <select
          value={selectedTerritoryId}
          onChange={(e) => setSelectedTerritoryId(e.target.value)}
          className="w-full border rounded px-3 py-2 text-sm"
        >
          <option value="" disabled>
            Vælg område…
          </option>
          {options.map((t) => (
            <option key={t.id} value={t.id}>
              {t.name}
            </option>
          ))}
        </select>
      </div>

      {error && <p className="text-sm text-red-600">{error}</p>}

      <button
        type="submit"
        disabled={!canSubmit}
        className="self-start bg-blue-600 text-white py-2 px-5 rounded disabled:opacity-40"
      >
        {isSubmitting ? "Overfører…" : "Overfør bestilling"}
      </button>
    </form>
  );
}
