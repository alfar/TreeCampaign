import { useOutletContext } from "react-router-dom";
import { useState } from "react";
import { PickupForm } from "./PickupForm";
import { StopActionButtons } from "./StopActionButtons";
import Button from "../../components/Button";
import type { TeamScreenContext } from "./TeamScreen";

export default function TeamStopsTab() {
  const { stops, team, campaign, queueAction, queueTeamAction, queueAdjustExtraTrees, refresh } =
    useOutletContext<TeamScreenContext>();
  const [activeStop, setActiveStop] = useState<string | null>(null);
  const [showPickupForm, setShowPickupForm] = useState(false);

  const visibleStops = stops.filter((s) => s.stopType !== "Delivered");
  const hasCollected = stops.some((s) => s.stopType === "Collected");

  return (
    <div className="m-4 flex flex-col gap-4">
      {team && (
        <div className="flex items-center justify-center gap-4 border rounded-xl p-3 bg-gray-50">
          <Button
            size="lg"
            className="px-4"
            disabled={team.currentExtraTrees <= 0}
            onClick={() => queueAdjustExtraTrees(-1)}
          >
            −
          </Button>
          <span className="text-lg font-semibold">
            {team.currentExtraTrees} ekstra træer
          </span>
          <Button
            size="lg"
            className="px-4"
            onClick={() => queueAdjustExtraTrees(1)}
          >
            +
          </Button>
        </div>
      )}

      {team?.kind === "Trailer" && (
        <div className="flex gap-2">
          <Button
            size="lg"
            className="flex-1 bg-orange-500 hover:bg-orange-600"
            disabled={team.isTrailerFull === true}
            onClick={() => queueTeamAction("reportTrailerFull")}
          >
            {team.isTrailerFull ? "Trailer fuld ✓" : "Trailer fuld"}
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

      {showPickupForm && campaign && team && (
        <PickupForm
          campaign={campaign}
          teamId={team.id}
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
              {activeStop === stop.id && (
                <StopActionButtons stop={stop} queueAction={queueAction} />
              )}
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
              {activeStop === stop.id && (
                <StopActionButtons stop={stop} queueAction={queueAction} />
              )}
            </li>
          ))}
      </ol>
    </div>
  );
}
