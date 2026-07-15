import Button from "../../components/Button";
import { reopenStop, unassignStop } from "../../shared/api/client";
import type { Stop } from "../../shared/api/models/stop";
import {
  CheckIcon,
  ExclamationTriangleIcon,
  MapPinIcon,
  QuestionMarkCircleIcon,
} from "@heroicons/react/24/outline";

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
      case "Unassigned":
        return <MapPinIcon className="w-8 h-8 text-blue-600" />;
      case "Assigned":
        return <QuestionMarkCircleIcon className="w-8 h-8 text-blue-600" />;
      case "Unresolved":
        return <ExclamationTriangleIcon className="w-8 h-8 text-red-600" />;
      case "Collected":
        return <CheckIcon className="w-8 h-8 text-green-600" />;
      case "Delivered":
        return <CheckIcon className="w-8 h-8 text-gray-200" />;
      default:
        return null;
    }
  };

  const getStopButtons = (stop: Stop) => {
    if (stop.stopType === "Assigned") {
      return (
        <Button
          className="flex-1 bg-green-600"
          onClick={() => unassignStop(campaignId, stop.id).then(onUpdateStop)}
        >
          Fjern
        </Button>
      );
    } else if (stop.stopType === "Unresolved") {
      return (
        <Button
          className="flex-1 bg-green-600"
          onClick={() => reopenStop(campaignId, stop.id).then(onUpdateStop)}
        >
          Genåbn
        </Button>
      );
    } else if (stop.stopType === "Collected") {
      return (
        <Button
          className="flex-1 bg-green-600"
          onClick={() => reopenStop(campaignId, stop.id).then(onUpdateStop)}
        >
          Genåbn
        </Button>
      );
    }
  };

  return (
    <label
      htmlFor={assignMode ? stop.id : undefined}
      className={"p-2 border rounded flex flex-row items-center gap-2 " + (selected ? "border-blue-200 bg-blue-100" : "border-gray-200")}
    >
      {assignMode && (
        <input
          type="checkbox"
          id={stop.id}
          className="w-6 h-6"
          checked={selected}
          onChange={() => onToggleSelect && onToggleSelect(stop.id)}
        />
      )}
      <div className="w-8">{getStopIcon(stop.stopType)}</div>
      <div className="w-full">
        <div className="flex justify-between">
          <h2 className="text-sm">{stop.address.displayName}</h2>
          <div className="text-sm">{stop.amount}</div>
        </div>
        <div>{getStopButtons(stop)}</div>
      </div>
    </label>
  );
}
