import { useState } from "react";
import { createTeam } from "../../shared/api/client";
import type { Team, TeamKind } from "../../shared/api/models/team";

interface CreateTeamFormProps {
  campaignId: string;
  onCreated: (team: Team) => void;
  onCancel: () => void;
}

export default function CreateTeamForm({ campaignId, onCreated, onCancel }: CreateTeamFormProps) {
  const [name, setName] = useState("");
  const [kind, setKind] = useState<TeamKind>("Walking");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const canSubmit = name.trim().length > 0 && !isSubmitting;

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (!canSubmit) return;

    setIsSubmitting(true);
    setError(null);
    try {
      const team = await createTeam(campaignId, name.trim(), kind);
      setName("");
      onCreated(team);
    } catch {
      setError("Noget gik galt. Prøv igen.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-3 p-4 border rounded bg-gray-50">
      <h2 className="text-base font-semibold">Nyt hold</h2>
      <div>
        <label className="text-sm font-medium text-gray-700 block mb-1">Navn</label>
        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          className="w-full border rounded px-3 py-2 text-sm"
          placeholder="Hold A"
          autoFocus
        />
      </div>
      <div>
        <label className="text-sm font-medium text-gray-700 block mb-1">Type</label>
        <div className="flex gap-2">
          <button
            type="button"
            onClick={() => setKind("Walking")}
            className={`flex-1 py-2 rounded text-sm border ${kind === "Walking" ? "bg-blue-600 text-white border-blue-600" : "border-gray-300"}`}
          >
            Gående
          </button>
          <button
            type="button"
            onClick={() => setKind("Trailer")}
            className={`flex-1 py-2 rounded text-sm border ${kind === "Trailer" ? "bg-blue-600 text-white border-blue-600" : "border-gray-300"}`}
          >
            Trailer
          </button>
        </div>
      </div>
      {error && <p className="text-sm text-red-600">{error}</p>}
      <div className="flex gap-2">
        <button
          type="submit"
          disabled={!canSubmit}
          className="bg-blue-600 text-white py-2 px-5 rounded text-sm disabled:opacity-40"
        >
          {isSubmitting ? "Opretter…" : "Opret hold"}
        </button>
        <button type="button" onClick={onCancel} className="py-2 px-5 rounded border text-sm">
          Annuller
        </button>
      </div>
    </form>
  );
}
