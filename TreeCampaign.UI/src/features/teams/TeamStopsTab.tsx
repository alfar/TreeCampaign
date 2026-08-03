import { useParams } from "react-router-dom";
import {
  collectStop,
  correctStop,
  deliverLoad,
  getCampaign,
  getStopsForTeam,
  getTeam,
  markStopUnresolved,
  reportTrailerFull,
  retryStop,
} from "../../shared/api/client";
import { useEffect, useState } from "react";
import type { Campaign } from "../../shared/api/models/campagin";
import type { Stop } from "../../shared/api/models/stop";
import type { Team } from "../../shared/api/models/team";
import { PickupForm } from "./PickupForm";
import Button from "../../components/Button";

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
      getTeam(campaignId, teamId).then(setTeam);
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
            <Button
              size="lg"
              className="flex-1 bg-green-600 hover:bg-green-700"
              onClick={() =>
                collectStop(campaignId, stop.id).then((newStop) =>
                  updateStop(newStop),
                )
              }
            >
              Hentet
            </Button>
            <Button
              variant="danger"
              size="lg"
              className="flex-1"
              onClick={() =>
                markStopUnresolved(campaignId, stop.id).then((newStop) =>
                  updateStop(newStop),
                )
              }
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
              onClick={() =>
                retryStop(campaignId, stop.id).then((newStop) =>
                  updateStop(newStop),
                )
              }
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
              onClick={() =>
                correctStop(campaignId, stop.id).then((newStop) =>
                  updateStop(newStop),
                )
              }
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
      {team?.kind === "Trailer" && (
        <div className="flex gap-2">
          <Button
            size="lg"
            className="flex-1 bg-orange-500 hover:bg-orange-600"
            onClick={() => reportTrailerFull(campaignId, teamId)}
          >
            Trailer fuld
          </Button>
          {hasCollected && (
            <Button
              size="lg"
              className="flex-1 bg-green-700 hover:bg-green-800"
              onClick={() =>
                deliverLoad(campaignId, teamId).then(() =>
                  getStopsForTeam(campaignId, teamId).then(setStops),
                )
              }
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
