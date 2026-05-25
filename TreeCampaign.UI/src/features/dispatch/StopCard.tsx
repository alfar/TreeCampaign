import { reopenStop, unassignStop } from "../../shared/api/client";
import type { Stop } from "../../shared/api/models/stop";
import {
  CheckIcon,
  ExclamationTriangleIcon,
  QuestionMarkCircleIcon,
} from "@heroicons/react/24/solid";

interface StopCardProps {
  campaignId: string;
  stop: Stop;
  assignMode: boolean;
  selected?: boolean;
  onToggleSelect?: (stopId: string) => any;
  onUpdateStop?: (stop: Stop) => any;
}

export default function StopCard({
  campaignId,
  stop,
  selected,
  assignMode,
  onToggleSelect,
  onUpdateStop,
}: StopCardProps) {
  const getStopIcon = (stopType: string) => {
    switch (stopType) {
      case "Assigned":
        return <QuestionMarkCircleIcon className="w-10 h-10 text-blue-600" />;
      case "Unresolved":
        return <ExclamationTriangleIcon className="w-6 h-6 text-red-600" />;
      case "Collected":
        return <CheckIcon className="w-6 h-6 text-green-600" />;
      default:
        return null;
    }
  };

  const getStopButtons = (stop: Stop) => {
    if (stop.stopType === "Assigned") {
      return (
        <button
          className="flex-1 bg-green-600 text-white py-3 rounded-xl"
          onClick={() => unassignStop(campaignId, stop.id).then(onUpdateStop)}
        >
          Fjern
        </button>
      );
    } else if (stop.stopType === "Unresolved") {
      return (
        <button
          className="flex-1 bg-green-600 text-white py-3 rounded-xl"
          onClick={() => reopenStop(campaignId, stop.id).then(onUpdateStop)}
        >
          Genåbn
        </button>
      );
    } else if (stop.stopType === "Collected") {
      return (
        <button
          className="flex-1 bg-green-600 text-white py-3 rounded-xl"
          onClick={() => reopenStop(campaignId, stop.id).then(onUpdateStop)}
        >
          Genåbn
        </button>
      );
    }
  };

  return (
    <div
      key={stop.id}
      className="p-2 border rounded flex flex-row items-center gap-2"
    >
      {assignMode ? (
        <input
          type="checkbox"
          className="w-6 h-6"
          checked={selected}
          onChange={() => onToggleSelect && onToggleSelect(stop.id)}
        />
      ) : (
        <div className="">{getStopIcon(stop.stopType)}</div>
      )}
      <div>
        <h2 className="text-lg font-semibold">{stop.address.displayName}</h2>
        <p>{stop.amount}</p>
        {getStopButtons(stop)}
      </div>
    </div>
  );
}
