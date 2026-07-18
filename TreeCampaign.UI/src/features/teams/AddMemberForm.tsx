import { useState } from "react";

export function AddMemberForm({ onAdd }: { onAdd: (name: string, phoneNumber: string, scoutRelativeName?: string) => Promise<void>; }) {
  const [name, setName] = useState("");
  const [phone, setPhone] = useState("");
  const [scout, setScout] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim() || !phone.trim()) return;
    setLoading(true);
    await onAdd(name.trim(), phone.trim(), scout.trim() || undefined);
    setName("");
    setPhone("");
    setScout("");
    setLoading(false);
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-2 border rounded p-3">
      <p className="font-medium text-sm">Tilføj patruljemedlem</p>
      <input
        className="border rounded px-2 py-1 text-sm"
        placeholder="Navn"
        value={name}
        onChange={(e) => setName(e.target.value)}
        required />
      <input
        className="border rounded px-2 py-1 text-sm"
        placeholder="Telefon"
        value={phone}
        onChange={(e) => setPhone(e.target.value)}
        required />
      <input
        className="border rounded px-2 py-1 text-sm"
        placeholder="Spejderslægtning (valgfrit)"
        value={scout}
        onChange={(e) => setScout(e.target.value)} />
      <button
        type="submit"
        disabled={loading || !name.trim() || !phone.trim()}
        className="bg-blue-600 text-white rounded px-3 py-1 text-sm disabled:opacity-50"
      >
        {loading ? "Tilføjer…" : "Tilføj"}
      </button>
    </form>
  );
}
