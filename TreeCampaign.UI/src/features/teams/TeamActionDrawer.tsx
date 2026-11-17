import { useState, type ReactNode } from "react";
import { ChevronDownIcon, ChevronUpIcon } from "@heroicons/react/24/outline";
import { TeamActionPanel } from "./TeamActionPanel";
import type { TeamScreenContext } from "./TeamScreen";

interface TeamActionDrawerProps {
  team: NonNullable<TeamScreenContext["team"]>;
  hasCollected: boolean;
  queueTeamAction: TeamScreenContext["queueTeamAction"];
  queueAdjustExtraTrees: TeamScreenContext["queueAdjustExtraTrees"];
  /** "fixed" pins to the viewport, "absolute" pins to the nearest positioned parent (e.g. the map wrapper). */
  position?: "fixed" | "absolute";
  /** Extra content shown inside the expanded drawer, below the standard actions. */
  children?: ReactNode;
}

/** Collapsible bottom drawer with the team's extra-trees counter and trailer actions. */
export function TeamActionDrawer({
  team,
  hasCollected,
  queueTeamAction,
  queueAdjustExtraTrees,
  position = "fixed",
  children,
}: TeamActionDrawerProps) {
  const [open, setOpen] = useState(false);

  return (
    <div
      className={`${position} bottom-0 left-0 right-0 z-1000 rounded-t-2xl bg-white shadow-[0_-4px_16px_rgba(0,0,0,0.2)]`}
      style={{ paddingBottom: "env(safe-area-inset-bottom)" }}
    >
      <button
        type="button"
        className="relative flex w-full items-center justify-between gap-2 px-4 pt-4 pb-3 min-h-11"
        aria-expanded={open}
        onClick={() => setOpen((v) => !v)}
      >
        <span className="absolute left-1/2 -translate-x-1/2 top-1.5 h-1 w-10 rounded-full bg-gray-300" />
        <span className="text-sm font-semibold text-gray-700">
          Handlinger · {team.currentExtraTrees} ekstra træer
          {team.isTrailerFull ? " · Trailer fuld" : ""}
        </span>
        {open ? (
          <ChevronDownIcon className="h-5 w-5 text-gray-500" />
        ) : (
          <ChevronUpIcon className="h-5 w-5 text-gray-500" />
        )}
      </button>
      {open && (
        <div className="flex flex-col gap-2 px-3 pb-3">
          <TeamActionPanel
            team={team}
            hasCollected={hasCollected}
            queueTeamAction={queueTeamAction}
            queueAdjustExtraTrees={queueAdjustExtraTrees}
          />
          {children}
        </div>
      )}
    </div>
  );
}
