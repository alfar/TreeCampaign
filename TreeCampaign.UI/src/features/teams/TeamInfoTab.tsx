import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import QRCode from "react-qr-code";
import { addTeamMember, getTeams, removeTeamMember } from "../../shared/api/client";
import type { Team } from "../../shared/api/models/team";
import UpdateTeamForm from "./UpdateTeamForm";
import { AddMemberForm } from "./AddMemberForm";
import { MemberRow } from "./MemberRow";

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

  const handleAddMember = async (name: string, phoneNumber?: string, scoutRelativeName?: string) => {
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
