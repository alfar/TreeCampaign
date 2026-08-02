import { useState } from "react";
import { createUser } from "../../shared/api/client";
import type { User } from "../../shared/api/models/user";

interface CreateUserFormProps {
  onCreated: (user: User) => void;
}

export default function CreateUserForm({ onCreated }: CreateUserFormProps) {
  const [email, setEmail] = useState("");
  const [displayName, setDisplayName] = useState("");
  const [password, setPassword] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const canSubmit = email.trim() !== "" && displayName.trim() !== "" && password.trim() !== "" && !isSubmitting;

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (!canSubmit) return;

    setIsSubmitting(true);
    setError(null);
    try {
      const res = await createUser(email, displayName, password);
      if (!res.ok) {
        setError("Noget gik galt. Tjek oplysningerne og prøv igen.");
        return;
      }

      const created: User = await res.json();
      setEmail("");
      setDisplayName("");
      setPassword("");
      onCreated(created);
    } catch {
      setError("Noget gik galt. Prøv igen.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4 p-4 border rounded bg-gray-50">
      <h2 className="text-base font-semibold">Ny bruger</h2>
      <div>
        <label className="text-sm font-medium text-gray-700 block mb-1">Navn</label>
        <input
          type="text"
          value={displayName}
          onChange={(e) => setDisplayName(e.target.value)}
          className="w-full border rounded px-3 py-2 text-sm"
          autoFocus
          required
        />
      </div>
      <div>
        <label className="text-sm font-medium text-gray-700 block mb-1">Email</label>
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full border rounded px-3 py-2 text-sm"
          required
        />
      </div>
      <div>
        <label className="text-sm font-medium text-gray-700 block mb-1">Adgangskode</label>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="w-full border rounded px-3 py-2 text-sm"
          required
        />
      </div>
      {error && <p className="text-sm text-red-600">{error}</p>}
      <button
        type="submit"
        disabled={!canSubmit}
        className="self-start bg-blue-600 text-white py-2 px-5 rounded disabled:opacity-40"
      >
        {isSubmitting ? "Opretter…" : "Opret bruger"}
      </button>
    </form>
  );
}
