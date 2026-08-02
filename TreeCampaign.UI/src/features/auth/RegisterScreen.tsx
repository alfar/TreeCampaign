import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../app/AuthContext";
import { registerScoutGroup } from "../../shared/api/client";

export default function RegisterScreen() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [groupName, setGroupName] = useState("");
  const [ownerDisplayName, setOwnerDisplayName] = useState("");
  const [ownerEmail, setOwnerEmail] = useState("");
  const [ownerPassword, setOwnerPassword] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (isSubmitting) return;

    setIsSubmitting(true);
    setError(null);
    try {
      const res = await registerScoutGroup(groupName, ownerEmail, ownerDisplayName, ownerPassword);
      if (!res.ok) {
        setError("Noget gik galt. Tjek dine oplysninger og prøv igen.");
        return;
      }

      const success = await login(ownerEmail, ownerPassword);
      navigate(success ? "/" : "/login", { replace: true });
    } catch {
      setError("Noget gik galt. Prøv igen.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-svh bg-blue-800">
      <form
        onSubmit={handleSubmit}
        className="flex flex-col gap-4 p-8 border rounded bg-white w-full max-w-sm"
      >
        <div className="rounded-full bg-blue-600 w-20 h-20 flex items-center justify-center self-center mb-4">
          <img src="/logo.png" alt="TreeCampaign logo" className="w-12 self-center" />
        </div>
        <h1 className="text-lg font-semibold text-center">Opret gruppe</h1>
        <div>
          <label className="text-sm font-medium text-gray-700 block mb-1">Gruppenavn</label>
          <input
            type="text"
            value={groupName}
            onChange={(e) => setGroupName(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
            autoFocus
            required
          />
        </div>
        <div>
          <label className="text-sm font-medium text-gray-700 block mb-1">Dit navn</label>
          <input
            type="text"
            value={ownerDisplayName}
            onChange={(e) => setOwnerDisplayName(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
            required
          />
        </div>
        <div>
          <label className="text-sm font-medium text-gray-700 block mb-1">Email</label>
          <input
            type="email"
            value={ownerEmail}
            onChange={(e) => setOwnerEmail(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
            required
          />
        </div>
        <div>
          <label className="text-sm font-medium text-gray-700 block mb-1">Adgangskode</label>
          <input
            type="password"
            value={ownerPassword}
            onChange={(e) => setOwnerPassword(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
            required
          />
        </div>
        {error && <p className="text-sm text-red-600">{error}</p>}
        <button
          type="submit"
          disabled={isSubmitting}
          className="bg-blue-600 text-white py-2 px-5 rounded disabled:opacity-40"
        >
          {isSubmitting ? "Opretter…" : "Opret gruppe"}
        </button>
        <Link to="/login" className="text-sm text-blue-600 text-center hover:underline">
          Har du allerede en konto? Log ind
        </Link>
      </form>
    </div>
  );
}
