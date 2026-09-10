import { useParams } from "react-router-dom";
import { getCampaign, getTeam } from "../../shared/api/client";
import { useEffect, useState } from "react";
import type { Campaign } from "../../shared/api/models/campagin";
import type { Team } from "../../shared/api/models/team";
import { PickupForm } from "./PickupForm";
import Button from "../../components/Button";
import { useTeamStops } from "../../shared/offline/useTeamStops";
import { SignalSlashIcon } from "@heroicons/react/24/outline";

export default function TeamStopsTab() {
  const params = useParams();
  const campaignId = params.campaignId!;
  const teamId = params.teamId!;

  const { stops, isTrailerFull, isOffline, pendingCount, queueAction, queueTeamAction, refresh } =
    useTeamStops(campaignId, teamId);
  const [team, setTeam] = useState<Team | null>(null);
  const [campaign, setCampaign] = useState<Campaign | null>(null);
  const [activeStop, setActiveStop] = useState<string | null>(null);
  const [showPickupForm, setShowPickupForm] = useState(false);
  const [showOfflineDetails, setShowOfflineDetails] = useState(false);

  useEffect(() => {
    if (campaignId) {
      getTeam(campaignId, teamId).then(setTeam);
      getCampaign(campaignId).then(setCampaign);
    }
  }, [campaignId, teamId]);

  function getStopButtons(stop: (typeof stops)[number]) {
    if (activeStop === stop.id) {
      if (stop.stopType === "Assigned") {
        return (
          <div className="flex gap-2 mt-4">
            <Button
              size="lg"
              className="flex-1 bg-green-600 hover:bg-green-700"
              onClick={() => queueAction(stop.id, "collect")}
            >
              Hentet
            </Button>
            <Button
              variant="danger"
              size="lg"
              className="flex-1"
              onClick={() => queueAction(stop.id, "unresolved", "Ikke fundet")}
            >
              Ikke fundet
            </Button>
          </div>
        );
      } else if (stop.stopType === "Unresolved") {
        return (
          <div className="flex gap-2 mt-4">
            <Button
              size="lg"
              className="flex-1 bg-green-600 hover:bg-green-700"
              onClick={() => queueAction(stop.id, "retry")}
            >
              Genoptag
            </Button>
          </div>
        );
      } else if (stop.stopType === "Collected") {
        return (
          <div className="flex gap-2 mt-4">
            <Button
              variant="danger"
              size="lg"
              className="flex-1"
              onClick={() => queueAction(stop.id, "correct")}
            >
              Fortryd
            </Button>
          </div>
        );
      }
    }
    return null;
  }

  const visibleStops = stops.filter((s) => s.stopType !== "Delivered");
  const hasCollected = stops.some((s) => s.stopType === "Collected");

  return (
    <div className="m-4 flex flex-col gap-4">
      {isOffline && (
        <div className="relative flex justify-end">
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
              {pendingCount > 0 &&
                ` — ${pendingCount} handling${pendingCount === 1 ? "" : "er"} venter på at blive sendt`}
            </div>
          )}
        </div>
      )}

      {team?.kind === "Trailer" && (
        <div className="flex gap-2">
          <Button
            size="lg"
            className="flex-1 bg-orange-500 hover:bg-orange-600"
            disabled={isTrailerFull === true}
            onClick={() => queueTeamAction("reportTrailerFull")}
          >
            {isTrailerFull ? "Trailer fuld ✓" : "Trailer fuld"}
          </Button>
          {hasCollected && (
            <Button
              size="lg"
              className="flex-1 bg-green-700 hover:bg-green-800"
              onClick={() => queueTeamAction("deliverLoad")}
            >
              Lever last
            </Button>
          )}
        </div>
      )}

      {team?.kind === "Walking" && (
        <Button
          size="lg"
          className="w-full"
          onClick={() => setShowPickupForm((v) => !v)}
        >
          {showPickupForm ? "Annuller afhentning" : "Anmod om afhentning"}
        </Button>
      )}

      {showPickupForm && campaign && (
        <PickupForm
          campaign={campaign}
          onCreated={() => {
            refresh();
            setShowPickupForm(false);
          }}
        />
      )}

      <ol className="flex flex-col gap-2">
        {visibleStops
          .filter((stop) => stop.stopType === "Assigned")
          .map((stop) => (
            <li
              key={stop.id}
              className={
                activeStop === stop.id
                  ? "p-4 border rounded bg-blue-100"
                  : "p-4 border rounded"
              }
              onClick={() => setActiveStop(stop.id)}
            >
              <h2 className="text-lg font-semibold">
                {stop.address.displayName}
              </h2>
              <p>{stop.amount}</p>
              {getStopButtons(stop)}
            </li>
          ))}
        {visibleStops
          .filter((stop) => stop.stopType !== "Assigned")
          .map((stop) => (
            <li
              key={stop.id}
              className={
                activeStop === stop.id
                  ? "p-4 border rounded bg-blue-100"
                  : "p-4 border border-gray-200 text-gray-300 rounded"
              }
              onClick={() => setActiveStop(stop.id)}
            >
              <h2 className="text-lg font-semibold">
                {stop.address.displayName}
              </h2>
              <p>{stop.amount}</p>
              {getStopButtons(stop)}
            </li>
          ))}
      </ol>
    </div>
  );
}
