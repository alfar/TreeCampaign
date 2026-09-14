import { useEffect } from "react";
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

const SILKEBORG_CENTER: [number, number] = [56.1697, 9.5451];

const STOP_COLORS: Record<string, string> = {
  Assigned: "#2563eb",
  Unresolved: "#dc2626",
  Collected: "#16a34a",
};

function FitBoundsToStops({ stops }: { stops: Stop[] }) {
  const map = useMap();

  useEffect(() => {
    if (stops.length === 0) return;
    const bounds: [number, number][] = stops.map((s) => [
      s.address.latitude,
      s.address.longitude,
    ]);
    map.fitBounds(bounds, { padding: [32, 32] });
  }, [stops, map]);

  return null;
}

export default function TeamMapTab() {
  const { stops, team, queueAdjustExtraTrees, queueAction } = useOutletContext<TeamScreenContext>();

  const visibleStops = stops.filter((s) => s.stopType !== "Delivered");

  return (
    <div className="relative h-[calc(100vh-4rem)] w-full">
      {team && (
        <div className="absolute z-1000 top-3 left-1/2 -translate-x-1/2 flex items-center gap-3 border rounded-xl px-3 py-2 bg-white shadow">
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
        </div>
      )}
      <MapContainer
        center={SILKEBORG_CENTER}
        zoom={13}
        className="h-full w-full"
      >
        <TileLayer
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        <FitBoundsToStops stops={visibleStops} />
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
