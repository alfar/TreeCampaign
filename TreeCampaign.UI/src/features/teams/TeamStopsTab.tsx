import { useParams } from "react-router-dom";
import {
  collectStop,
  correctStop,
  deliverLoad,
  getCampaign,
  getStopsForTeam,
  getTeams,
  markStopUnresolved,
  reportTrailerFull,
  retryStop,
} from "../../shared/api/client";
import { useEffect, useState } from "react";
import type { Campaign } from "../../shared/api/models/campagin";
import type { Stop } from "../../shared/api/models/stop";
import type { Team } from "../../shared/api/models/team";
import { PickupForm } from "./PickupForm";

export default function TeamStopsTab() {
  const params = useParams();
  const campaignId = params.campaignId!;
  const teamId = params.teamId!;

  const [stops, setStops] = useState<Stop[]>([]);
  const [team, setTeam] = useState<Team | null>(null);
  const [campaign, setCampaign] = useState<Campaign | null>(null);
  const [activeStop, setActiveStop] = useState<string | null>(null);
  const [showPickupForm, setShowPickupForm] = useState(false);

  useEffect(() => {
    if (campaignId) {
      getStopsForTeam(campaignId, teamId).then(setStops);
      getTeams(campaignId).then((teams) =>
        setTeam(teams.find((t) => t.id === teamId) ?? null),
      );
      getCampaign(campaignId).then(setCampaign);
    }
  }, [campaignId, teamId]);

  function updateStop(stop: Stop) {
    setStops((prevStops) =>
      prevStops.map((s) => (s.id === stop.id ? stop : s)),
    );
  }

  function getStopButtons(stop: Stop) {
    if (activeStop === stop.id) {
      if (stop.stopType === "Assigned") {
        return (
          <div className="flex gap-2 mt-4">
            <button
              className="flex-1 bg-green-600 text-white py-3 rounded-xl"
              onClick={() =>
                collectStop(campaignId, stop.id).then((newStop) =>
                  updateStop(newStop),
                )
              }
            >
              Hentet
            </button>
            <button
              className="flex-1 bg-red-600 text-white py-3 rounded-xl"
              onClick={() =>
                markStopUnresolved(campaignId, stop.id).then((newStop) =>
                  updateStop(newStop),
                )
              }
            >
              Ikke fundet
            </button>
          </div>
        );
      } else if (stop.stopType === "Unresolved") {
        return (
          <div className="flex gap-2 mt-4">
            <button
              className="flex-1 bg-green-600 text-white py-3 rounded-xl"
              onClick={() =>
                retryStop(campaignId, stop.id).then((newStop) =>
                  updateStop(newStop),
                )
              }
            >
              Genoptag
            </button>
          </div>
        );
      } else if (stop.stopType === "Collected") {
        return (
          <div className="flex gap-2 mt-4">
            <button
              className="flex-1 bg-red-600 text-white py-3 rounded-xl"
              onClick={() =>
                correctStop(campaignId, stop.id).then((newStop) =>
                  updateStop(newStop),
                )
              }
            >
              Fortryd
            </button>
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
      {team?.kind === "Trailer" && (
        <div className="flex gap-2">
          <button
            className="flex-1 bg-orange-500 text-white py-3 rounded-xl font-medium"
            onClick={() => reportTrailerFull(campaignId, teamId)}
          >
            Trailer fuld
          </button>
          {hasCollected && (
            <button
              className="flex-1 bg-green-700 text-white py-3 rounded-xl font-medium"
              onClick={() =>
                deliverLoad(campaignId, teamId).then(() =>
                  getStopsForTeam(campaignId, teamId).then(setStops),
                )
              }
            >
              Lever last
            </button>
          )}
        </div>
      )}

      {team?.kind === "Walking" && (
        <button
          className="w-full bg-blue-600 text-white py-3 rounded-xl font-medium"
          onClick={() => setShowPickupForm((v) => !v)}
        >
          {showPickupForm ? "Annuller afhentning" : "Anmod om afhentning"}
        </button>
      )}

      {showPickupForm && campaign && (
        <PickupForm
          campaign={campaign}
          onCreated={(stop) => {
            setStops((prev) => [...prev, stop]);
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
