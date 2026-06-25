import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import QRCode from "react-qr-code";
import { getTeams } from "../../shared/api/client";
import type { Team } from "../../shared/api/models/team";
import UpdateTeamForm from "./UpdateTeamForm";

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

  return (
    <div className="flex flex-col gap-4 p-4">
      <UpdateTeamForm
        campaignId={campaignId!}
        team={team}
        onUpdated={setTeam}
      />
      <div className="flex justify-center">
        <QRCode value={location.href} />
      </div>
    </div>
  );
}
