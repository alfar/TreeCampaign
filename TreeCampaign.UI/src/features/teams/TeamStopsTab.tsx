import { useParams } from "react-router-dom";
import {
  collectStop,
  correctStop,
  getStopsForTeam,
  markStopUnresolved,
  retryStop,
} from "../../shared/api/client";
import { useEffect, useState } from "react";
import type { Stop } from "../../shared/api/models/stop";

export default function TeamStopsTab() {
  const params = useParams();
  const campaignId = params.campaignId!;
  const teamId = params.teamId!;

  const [stops, setStops] = useState<Stop[]>([]);
  const [activeStop, setActiveStop] = useState<string | null>(null);

  useEffect(() => {
    if (campaignId) {
      getStopsForTeam(campaignId, teamId).then(setStops);
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

  return (
    <ol className="flex flex-col gap-2 m-4">
      {stops
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
      {stops
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
  );
}
