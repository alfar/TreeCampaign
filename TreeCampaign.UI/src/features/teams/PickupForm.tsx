import { useState, useEffect } from "react";
import { getTerritory, requestPickup } from "../../shared/api/client";
import type { Campaign } from "../../shared/api/models/campagin";
import type { Stop } from "../../shared/api/models/stop";
import type { Team } from "../../shared/api/models/team";
import { AddressPicker, type Address } from "../../shared/components/AddressPicker";

export function PickupForm({
  campaign, teamId, onCreated,
}: {
  campaign: Campaign;
  teamId: string;
  onCreated: (stop: Stop, team: Team) => void;
}) {
  const [defaultZipCode, setDefaultZipCode] = useState("");
  const [address, setAddress] = useState<Address>({ zipCode: "", street: null, streetName: "", houseNumber: "", isValid: null });
  const [treeCount, setTreeCount] = useState(8);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (campaign.territoryId) {
      getTerritory(campaign.territoryId).then((territory) =>
        setDefaultZipCode(territory.defaultZipCode),
      );
    }
  }, [campaign.territoryId]);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!address.street || !address.houseNumber) return;
    setSubmitting(true);
    setError(null);
    try {
      const { stop, team } = await requestPickup(
        campaign.id,
        teamId,
        address.street.id,
        address.houseNumber,
        treeCount
      );
      onCreated(stop, team);
    } catch {
      setError("Adressen kunne ikke valideres. Prøv igen.");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="border rounded-xl p-4 flex flex-col gap-3 bg-gray-50"
    >
      <h3 className="font-semibold text-base">Anmod om afhentning</h3>

      <AddressPicker defaultZipCode={defaultZipCode} onChange={setAddress} />

      <div className="flex items-center gap-3">
        <label className="text-sm">Antal træer</label>
        <input
          type="number"
          min={1}
          value={treeCount}
          onChange={(e) => setTreeCount(Number(e.target.value))}
          className="border rounded-lg px-3 py-2 w-20" />
      </div>

      {error && <p className="text-red-600 text-sm">{error}</p>}

      <button
        type="submit"
        disabled={!address.street || !address.houseNumber || submitting}
        className="bg-blue-600 text-white py-2 rounded-xl disabled:opacity-50"
      >
        {submitting ? "Sender..." : "Send afhentningsanmodning"}
      </button>
    </form>
  );
}
