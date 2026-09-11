import Button from "../../components/Button";
import type { Stop } from "../../shared/api/models/stop";
import type { QueuedStopActionType } from "../../shared/offline/stopQueueStorage";

export function StopActionButtons({
  stop,
  queueAction,
}: {
  stop: Stop;
  queueAction: (stopId: string, type: QueuedStopActionType, reason?: string) => void;
}) {
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
  }

  if (stop.stopType === "Unresolved") {
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
  }

  if (stop.stopType === "Collected") {
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

  return null;
}
