import { useEffect, useRef } from "react";
import { useOutletContext } from "react-router-dom";
import {
  CircleMarker,
  MapContainer,
  Popup,
  TileLayer,
  useMap,
} from "react-leaflet";
import type { Stop } from "../../shared/api/models/stop";
import type { TeamScreenContext } from "./TeamScreen";
import { StopActionButtons } from "./StopActionButtons";
import Button from "../../components/Button";
import { ViewfinderCircleIcon } from "@heroicons/react/24/outline";

const SILKEBORG_CENTER: [number, number] = [56.1697, 9.5451];

const STOP_COLORS: Record<string, string> = {
  Assigned: "#2563eb",
  Unresolved: "#dc2626",
  Collected: "#16a34a",
};

function stopsToBounds(stops: Stop[]): [number, number][] {
  return stops.map((s) => [s.address.latitude, s.address.longitude]);
}

function FitBoundsOnce({ stops }: { stops: Stop[] }) {
  const map = useMap();
  const hasFitOnce = useRef(false);

  useEffect(() => {
    if (stops.length === 0) return;
    if (hasFitOnce.current) return;
    hasFitOnce.current = true;
    map.fitBounds(stopsToBounds(stops), { padding: [32, 32] });
  }, [stops, map]);

  return null;
}

function TopControls({
  stops,
  team,
  queueAdjustExtraTrees,
}: {
  stops: Stop[];
  team: TeamScreenContext["team"];
  queueAdjustExtraTrees: TeamScreenContext["queueAdjustExtraTrees"];
}) {
  const map = useMap();

  return (
    <div className="absolute z-1000 top-3 left-1/2 -translate-x-1/2 flex items-center gap-3 border rounded-xl px-3 py-2 bg-white shadow">
      {team && (
        <>
          <Button
            size="md"
            className="px-3"
            disabled={team.currentExtraTrees <= 0}
            onClick={() => queueAdjustExtraTrees(-1)}
          >
            −
          </Button>
          <span className="text-sm font-semibold whitespace-nowrap">
            {team.currentExtraTrees} ekstra træer
          </span>
          <Button size="md" className="px-3" onClick={() => queueAdjustExtraTrees(1)}>
            +
          </Button>
          <span className="w-px self-stretch bg-gray-200" />
        </>
      )}
      <Button
        size="md"
        className="px-3"
        disabled={stops.length === 0}
        onClick={() => map.fitBounds(stopsToBounds(stops), { padding: [32, 32] })}
        title="Centrer kort"
      >
        <ViewfinderCircleIcon className="h-5 w-5" />
      </Button>
    </div>
  );
}

export default function TeamMapTab() {
  const { stops, team, queueAdjustExtraTrees, queueAction } = useOutletContext<TeamScreenContext>();

  const visibleStops = stops.filter((s) => s.stopType !== "Delivered");

  return (
    <div className="relative h-[calc(100vh-4rem)] w-full">
      <MapContainer
        center={SILKEBORG_CENTER}
        zoom={13}
        className="h-full w-full"
      >
        <TileLayer
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        <FitBoundsOnce stops={visibleStops} />
        <TopControls
          stops={visibleStops}
          team={team}
          queueAdjustExtraTrees={queueAdjustExtraTrees}
        />
        {visibleStops.map((stop) => (
          <CircleMarker
            key={stop.id}
            center={[stop.address.latitude, stop.address.longitude]}
            radius={10}
            pathOptions={{
              color: STOP_COLORS[stop.stopType] ?? "#6b7280",
              fillColor: STOP_COLORS[stop.stopType] ?? "#6b7280",
              fillOpacity: 0.8,
            }}
          >
            <Popup minWidth={200}>
              <div className="font-semibold">{stop.address.displayName}</div>
              <div>{stop.amount} træer</div>
              <div>{stop.stopType}</div>
              <StopActionButtons stop={stop} queueAction={queueAction} />
            </Popup>
          </CircleMarker>
        ))}
      </MapContainer>
    </div>
  );
}
