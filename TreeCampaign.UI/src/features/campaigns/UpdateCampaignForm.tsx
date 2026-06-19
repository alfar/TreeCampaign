import { useEffect, useState } from "react";
import { updateCampaign, getTerritories } from "../../shared/api/client";
import type { Campaign } from "../../shared/api/models/campagin";
import type { Territory } from "../../shared/api/models/territory";

interface UpdateCampaignFormProps {
  campaign: Campaign;
  onUpdated: (campaign: Campaign) => void;
  onCancel: () => void;
}

export default function UpdateCampaignForm({ campaign, onUpdated, onCancel }: UpdateCampaignFormProps) {
  const [year, setYear] = useState(campaign.season);
  const [territoryId, setTerritoryId] = useState(campaign.territoryId ?? "");
  const [territories, setTerritories] = useState<Territory[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getTerritories().then(setTerritories);
  }, []);

  const canSubmit = year >= 2000 && year <= 2100 && !isSubmitting;

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (!canSubmit) return;

    setIsSubmitting(true);
    setError(null);
    try {
      const updated = await updateCampaign(campaign.id, year, territoryId || undefined);
      onUpdated(updated);
    } catch {
      setError("Noget gik galt. Prøv igen.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4 p-4 border rounded bg-gray-50 mt-2">
      <h2 className="text-base font-semibold">Rediger kampagne</h2>
      <div className="flex gap-3">
        <div className="w-28">
          <label className="text-sm font-medium text-gray-700 block mb-1">År</label>
          <input
            type="number"
            value={year}
            onChange={(e) => setYear(Number(e.target.value))}
            className="w-full border rounded px-3 py-2 text-sm"
            min={2000}
            max={2100}
          />
        </div>
        <div className="flex-1">
          <label className="text-sm font-medium text-gray-700 block mb-1">Territorium</label>
          <select
            value={territoryId}
            onChange={(e) => setTerritoryId(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm bg-white"
          >
            <option value="">— Intet territorium —</option>
            {territories.map((t) => (
              <option key={t.id} value={t.id}>{t.name}</option>
            ))}
          </select>
        </div>
      </div>
      {error && <p className="text-sm text-red-600">{error}</p>}
      <div className="flex gap-2">
        <button
          type="submit"
          disabled={!canSubmit}
          className="bg-blue-600 text-white py-2 px-5 rounded disabled:opacity-40"
        >
          {isSubmitting ? "Gemmer…" : "Gem ændringer"}
        </button>
        <button type="button" onClick={onCancel} className="py-2 px-5 rounded border text-sm">
          Annuller
        </button>
      </div>
    </form>
  );
}
