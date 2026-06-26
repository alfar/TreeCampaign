import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import QRCode from "react-qr-code";
import { addTeamMember, getTeams, removeTeamMember } from "../../shared/api/client";
import type { Team, TeamMember } from "../../shared/api/models/team";
import UpdateTeamForm from "./UpdateTeamForm";

function AddMemberForm({ onAdd }: { onAdd: (name: string, phoneNumber: string, scoutRelativeName?: string) => Promise<void> }) {
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
        required
      />
      <input
        className="border rounded px-2 py-1 text-sm"
        placeholder="Telefon"
        value={phone}
        onChange={(e) => setPhone(e.target.value)}
        required
      />
      <input
        className="border rounded px-2 py-1 text-sm"
        placeholder="Spejderslægtning (valgfrit)"
        value={scout}
        onChange={(e) => setScout(e.target.value)}
      />
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

function MemberRow({ member, onRemove }: { member: TeamMember; onRemove: () => void }) {
  return (
    <div className="flex items-center justify-between py-1 border-b last:border-0">
      <div className="text-sm">
        <span className="font-medium">{member.name}</span>
        {member.scoutRelativeName && (
          <span className="text-gray-500 ml-1">({member.scoutRelativeName})</span>
        )}
        <span className="text-gray-600 ml-2">{member.phoneNumber}</span>
      </div>
      <button
        onClick={onRemove}
        className="text-red-500 text-xs ml-2 hover:text-red-700"
      >
        Fjern
      </button>
    </div>
  );
}

export default function TeamInfoTab() {
  const { campaignId, teamId } = useParams();
  const [team, setTeam] = useState<Team | null>(null);

  useEffect(() => {
    if (campaignId) {
      getTeams(campaignId).then((teams) =>
        setTeam(teams.find((t) => t.id === teamId) ?? null),
      );
    }
  }, [campaignId, teamId]);

  if (!team) return null;

  const handleAddMember = async (name: string, phoneNumber: string, scoutRelativeName?: string) => {
    const updated = await addTeamMember(campaignId!, teamId!, name, phoneNumber, scoutRelativeName);
    setTeam(updated);
  };

  const handleRemoveMember = async (memberId: string) => {
    const updated = await removeTeamMember(campaignId!, teamId!, memberId);
    setTeam(updated);
  };

  return (
    <div className="flex flex-col gap-4 p-4">
      <UpdateTeamForm
        campaignId={campaignId!}
        team={team}
        onUpdated={setTeam}
      />

      <div>
        <h3 className="font-medium mb-2">Patruljemedlemmer</h3>
        {team.members.length === 0 ? (
          <p className="text-sm text-gray-500">Ingen registrerede patruljemedlemmer</p>
        ) : (
          <div className="border rounded p-2">
            {team.members.map((m) => (
              <MemberRow
                key={m.id}
                member={m}
                onRemove={() => handleRemoveMember(m.id)}
              />
            ))}
          </div>
        )}
        <div className="mt-2">
          <AddMemberForm onAdd={handleAddMember} />
        </div>
      </div>

      <div className="flex justify-center">
        <QRCode value={location.href} />
      </div>
    </div>
  );
}
