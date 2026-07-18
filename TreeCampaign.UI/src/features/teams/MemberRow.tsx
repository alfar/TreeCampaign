import type { TeamMember } from "../../shared/api/models/team";

export function MemberRow({ member, onRemove }: { member: TeamMember; onRemove: () => void; }) {
  return (
    <div className="flex items-center justify-between py-1 border-b last:border-0">
      <div className="text-sm">
        <span className="font-medium">{member.name}</span>
        {member.scoutRelativeName && (
          <span className="text-gray-500 ml-1">({member.scoutRelativeName})</span>
        )}
        {member.phoneNumber && (
          <span className="text-gray-600 ml-2">{member.phoneNumber}</span>
        )}
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
