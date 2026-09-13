import { NavLink, Outlet, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { ArrowPathIcon, SignalSlashIcon } from "@heroicons/react/24/outline";
import { getCampaign } from "../../shared/api/client";
import type { Campaign } from "../../shared/api/models/campagin";
import { useTeamData } from "../../shared/offline/useTeamData";

export interface TeamScreenContext extends ReturnType<typeof useTeamData> {
  campaign: Campaign | null;
}

export default function TeamScreen() {
  const { campaignId, teamId } = useParams();
  const base = `/campaigns/${campaignId}/teams/${teamId}`;

  const teamData = useTeamData(campaignId!, teamId!);
  const [campaign, setCampaign] = useState<Campaign | null>(null);
  const [showOfflineDetails, setShowOfflineDetails] = useState(false);
  const [isRefreshing, setIsRefreshing] = useState(false);

  const handleRefresh = async () => {
    setIsRefreshing(true);
    try {
      await teamData.refresh();
    } finally {
      setIsRefreshing(false);
    }
  };

  useEffect(() => {
    if (campaignId) {
      getCampaign(campaignId).then(setCampaign);
    }
  }, [campaignId]);

  const navLink = ({ isActive }: { isActive: boolean }) =>
    `flex-1 text-center py-2 text-sm font-medium ${
      isActive
        ? "border-b-2 border-blue-600 text-blue-600"
        : "text-gray-500 border-b-2 border-transparent"
    }`;

  const outletContext: TeamScreenContext = { ...teamData, campaign };

  return (
    <div>
      <div className="fixed top-0 w-full bg-white z-10">
        <nav className="flex items-center">
          <NavLink to={`${base}/stops`} className={navLink}>
            Stop
          </NavLink>
          <NavLink to={`${base}/map`} className={navLink}>
            Kort
          </NavLink>
          <NavLink to={`${base}/info`} className={navLink}>
            Info
          </NavLink>
          <button
            type="button"
            onClick={handleRefresh}
            disabled={isRefreshing}
            className="flex items-center justify-center w-8 h-8 mr-1 text-gray-500 hover:text-gray-700 disabled:opacity-50"
            aria-label="Opdater"
          >
            <ArrowPathIcon className={`w-4 h-4 ${isRefreshing ? "animate-spin" : ""}`} />
          </button>
          {teamData.isOffline && (
            <div className="relative flex items-center pr-3">
              <button
                type="button"
                onClick={() => setShowOfflineDetails((v) => !v)}
                className="flex items-center justify-center w-8 h-8 rounded-full bg-yellow-100 text-yellow-800 hover:bg-yellow-200"
                aria-label="Ingen forbindelse"
              >
                <SignalSlashIcon className="w-4 h-4" />
              </button>
              {showOfflineDetails && (
                <div className="absolute top-9 right-0 rounded bg-yellow-100 text-yellow-800 text-sm px-3 py-2 shadow whitespace-nowrap">
                  Ingen forbindelse
                  {teamData.pendingCount > 0 &&
                    ` — ${teamData.pendingCount} handling${teamData.pendingCount === 1 ? "" : "er"} venter på at blive sendt`}
                </div>
              )}
            </div>
          )}
        </nav>
      </div>
      <div className="mt-16">
        <Outlet context={outletContext} />
      </div>
    </div>
  );
}
