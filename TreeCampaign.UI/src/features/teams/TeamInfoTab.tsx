import { useOutletContext } from "react-router-dom";
import QRCode from "react-qr-code";
import UpdateTeamForm from "./UpdateTeamForm";
import { AddMemberForm } from "./AddMemberForm";
import { MemberRow } from "./MemberRow";
import type { TeamScreenContext } from "./TeamScreen";

export default function TeamInfoTab() {
  const { team, queueUpdateTeam, queueAddMember, queueRemoveMember } =
    useOutletContext<TeamScreenContext>();

  if (!team) return null;

  return (
    <div className="flex flex-col gap-4 p-4">
      <UpdateTeamForm team={team} onUpdate={queueUpdateTeam} />

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
                onRemove={() => queueRemoveMember(m.id)}
              />
            ))}
          </div>
        )}
        <div className="mt-2">
          <AddMemberForm onAdd={queueAddMember} />
        </div>
      </div>

      <div className="flex justify-center">
        <QRCode value={location.href} />
      </div>
    </div>
  );
}
