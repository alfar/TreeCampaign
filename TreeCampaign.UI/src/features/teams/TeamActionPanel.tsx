import { useState } from "react";
import Button from "../../components/Button";
import ConfirmDialog from "../../components/ConfirmDialog";
import type { TeamScreenContext } from "./TeamScreen";

type PendingAction = "reportTrailerFull" | "deliverLoad";

const dialogTexts: Record<PendingAction, { title: string; message: string }> = {
  reportTrailerFull: {
    title: "Trailer fuld",
    message: "Brug kun denne funktion, hvis du er nødt til at efterlade træer uafhentet",
  },
  deliverLoad: {
    title: "Lever last",
    message: "Brug først denne funktion, når du holder i kø til at læsse af",
  },
};

interface TeamActionPanelProps {
  team: NonNullable<TeamScreenContext["team"]>;
  hasCollected: boolean;
  queueTeamAction: TeamScreenContext["queueTeamAction"];
  queueAdjustExtraTrees: TeamScreenContext["queueAdjustExtraTrees"];
}

/** Extra-trees counter and trailer actions (with confirmation dialogs). Shared by the stops tab and the map drawer. */
export function TeamActionPanel({
  team,
  hasCollected,
  queueTeamAction,
  queueAdjustExtraTrees,
}: TeamActionPanelProps) {
  const [pendingAction, setPendingAction] = useState<PendingAction | null>(null);

  return (
    <>
      <ConfirmDialog
        open={pendingAction !== null}
        title={pendingAction ? dialogTexts[pendingAction].title : ""}
        message={pendingAction ? dialogTexts[pendingAction].message : ""}
        confirmLabel="Fortsæt"
        cancelLabel="Afbryd"
        confirmClassName={
          pendingAction === "deliverLoad"
            ? "bg-green-700 hover:bg-green-800"
            : "bg-orange-500 hover:bg-orange-600"
        }
        onCancel={() => setPendingAction(null)}
        onConfirm={() => {
          if (pendingAction) queueTeamAction(pendingAction);
          setPendingAction(null);
        }}
      />

      <div className="flex items-center justify-center gap-4">
        <Button
          size="lg"
          className="px-4"
          disabled={team.currentExtraTrees <= 0}
          onClick={() => queueAdjustExtraTrees(-1)}
        >
          −
        </Button>
        <span className="text-lg font-semibold">{team.currentExtraTrees} ekstra træer</span>
        <Button size="lg" className="px-4" onClick={() => queueAdjustExtraTrees(1)}>
          +
        </Button>
      </div>

      {team.kind === "Trailer" && (
        <div className="flex gap-2">
          <Button
            size="lg"
            className="flex-1 bg-orange-500 hover:bg-orange-600"
            disabled={team.isTrailerFull === true}
            onClick={() => setPendingAction("reportTrailerFull")}
          >
            {team.isTrailerFull ? "Trailer fuld ✓" : "Trailer fuld"}
          </Button>
          {hasCollected && (
            <Button
              size="lg"
              className="flex-1 bg-green-700 hover:bg-green-800"
              onClick={() => setPendingAction("deliverLoad")}
            >
              Lever last
            </Button>
          )}
        </div>
      )}
    </>
  );
}
