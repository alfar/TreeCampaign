import { useOutletContext } from "react-router-dom";
import { useState } from "react";
import { PickupForm } from "./PickupForm";
import { StopActionButtons } from "./StopActionButtons";
import Button from "../../components/Button";
import { TeamActionDrawer } from "./TeamActionDrawer";
import type { TeamScreenContext } from "./TeamScreen";

export default function TeamStopsTab() {
  const { stops, team, campaign, queueAction, queueTeamAction, queueAdjustExtraTrees, refresh } =
    useOutletContext<TeamScreenContext>();
  const [activeStop, setActiveStop] = useState<string | null>(null);
  const [showPickupForm, setShowPickupForm] = useState(false);

  const visibleStops = stops.filter((s) => s.stopType !== "Delivered");
  const hasCollected = stops.some((s) => s.stopType === "Collected") || (team?.currentExtraTrees ?? 0) > 0;

  if (team?.status === "OnBreak") {
    return (
      <div className="flex items-center justify-center h-[calc(100vh-4rem)] text-3xl font-semibold text-gray-400">
        Pause
      </div>
    );
  }

  return (
    <div className="m-4 mb-24 flex flex-col gap-4">
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

      {team && (
        <TeamActionDrawer
          team={team}
          hasCollected={hasCollected}
          queueTeamAction={queueTeamAction}
          queueAdjustExtraTrees={queueAdjustExtraTrees}
        >
          {team.kind === "Walking" && (
            <Button
              size="lg"
              className="w-full"
              onClick={() => setShowPickupForm((v) => !v)}
            >
              {showPickupForm ? "Annuller afhentning" : "Anmod om afhentning"}
            </Button>
          )}
        </TeamActionDrawer>
      )}
    </div>
  );
}
