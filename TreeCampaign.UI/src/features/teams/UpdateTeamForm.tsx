import { useState } from "react";
import { updateTeam } from "../../shared/api/client";
import { trailerSizeLabels, type Team, type TrailerSize } from "../../shared/api/models/team";

interface UpdateTeamFormProps {
  campaignId: string;
  team: Team;
  onUpdated: (team: Team) => void;
}

export default function UpdateTeamForm({ campaignId, team, onUpdated }: UpdateTeamFormProps) {
  const [name, setName] = useState(team.name);
  const [trailerSize, setTrailerSize] = useState<TrailerSize>(team.trailerSize ?? "Small");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const canSubmit =
    name.trim().length > 0 &&
    (name !== team.name || trailerSize !== team.trailerSize) &&
    !isSubmitting;

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (!canSubmit) return;

    setIsSubmitting(true);
    setError(null);
    try {
      const updated = await updateTeam(
        campaignId,
        team.id,
        name.trim(),
        team.kind === "Trailer" ? trailerSize : undefined,
      );
      onUpdated(updated);
    } catch {
      setError("Noget gik galt. Prøv igen.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4 p-4 border rounded bg-gray-50">
      <h2 className="text-base font-semibold">Rediger hold</h2>
      <div>
        <label className="text-sm font-medium text-gray-700 block mb-1">Navn</label>
        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          className="w-full border rounded px-3 py-2 text-sm"
        />
      </div>
      {team.kind === "Trailer" && (
        <div>
          <label className="text-sm font-medium text-gray-700 block mb-1">Trailerstørrelse</label>
          <div className="flex gap-2">
            {(Object.keys(trailerSizeLabels) as TrailerSize[]).map((size) => (
              <button
                key={size}
                type="button"
                onClick={() => setTrailerSize(size)}
                className={`flex-1 py-2 rounded text-sm border ${trailerSize === size ? "bg-blue-600 text-white border-blue-600" : "border-gray-300"}`}
              >
                {trailerSizeLabels[size]}
              </button>
            ))}
          </div>
        </div>
      )}
      {error && <p className="text-sm text-red-600">{error}</p>}
      <button
        type="submit"
        disabled={!canSubmit}
        className="bg-blue-600 text-white py-2 px-5 rounded disabled:opacity-40 self-start"
      >
        {isSubmitting ? "Gemmer…" : "Gem ændringer"}
      </button>
    </form>
  );
}
