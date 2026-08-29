import { useState } from "react";
import { importStreetSections } from "../../shared/api/client";
import type { StreetSectionImportSummary } from "../../shared/api/models/streetSectionImport";

interface ImportStreetSectionsFormProps {
  territoryId: string;
  onImported: () => void;
}

export default function ImportStreetSectionsForm({ territoryId, onImported }: ImportStreetSectionsFormProps) {
  const [file, setFile] = useState<File | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [summary, setSummary] = useState<StreetSectionImportSummary | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (!file) return;

    setIsSubmitting(true);
    setError(null);
    setSummary(null);
    try {
      const result = await importStreetSections(territoryId, file);
      setSummary(result);
      onImported();
    } catch {
      setError("Noget gik galt. Prøv igen.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4">
      <div>
        <label className="text-sm font-medium text-gray-700 block mb-1">Vejstykker CSV-fil</label>
        <input
          type="file"
          accept=".csv"
          onChange={(e) => setFile(e.target.files?.[0] ?? null)}
          className="w-full border rounded px-3 py-2 text-sm"
        />
      </div>

      {error && <p className="text-sm text-red-600">{error}</p>}

      <button
        type="submit"
        disabled={!file || isSubmitting}
        className="self-start bg-blue-600 text-white py-2 px-5 rounded disabled:opacity-40"
      >
        {isSubmitting ? "Importerer…" : "Importér"}
      </button>

      {summary && (
        <div className="border-t pt-4 flex flex-col gap-3 text-sm">
          <p className="font-medium text-green-700">
            {summary.importedCount} vejstykke(r) importeret
          </p>

          {summary.skippedExistingCount > 0 && (
            <p className="text-gray-700">
              {summary.skippedExistingCount} sprunget over (fandtes allerede)
            </p>
          )}

          {summary.failures.length > 0 && (
            <div>
              <p className="text-red-700 mb-1">{summary.failures.length} linje(r) kunne ikke læses</p>
              <ul className="text-xs text-red-600 list-disc list-inside">
                {summary.failures.map((f) => (
                  <li key={f.lineNumber}>
                    Linje {f.lineNumber}: {f.reason}
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      )}
    </form>
  );
}
