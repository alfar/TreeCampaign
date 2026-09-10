import { useState } from "react";

export function AddMemberForm({ onAdd }: { onAdd: (name: string, phoneNumber?: string, scoutRelativeName?: string) => void; }) {
  const [name, setName] = useState("");
  const [phone, setPhone] = useState("");
  const [scout, setScout] = useState("");

  const handleSubmit = (e: React.SubmitEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!name.trim()) return;
    onAdd(name.trim(), phone.trim() || undefined, scout.trim() || undefined);
    setName("");
    setPhone("");
    setScout("");
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
        placeholder="Telefon (valgfrit)"
        value={phone}
        onChange={(e) => setPhone(e.target.value)} />
      <input
        className="border rounded px-2 py-1 text-sm"
        placeholder="Spejderslægtning (valgfrit)"
        value={scout}
        onChange={(e) => setScout(e.target.value)} />
      <button
        type="submit"
        disabled={!name.trim()}
        className="bg-blue-600 text-white rounded px-3 py-1 text-sm disabled:opacity-50"
      >
        Tilføj
      </button>
    </form>
  );
}
