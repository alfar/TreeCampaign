import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  CircleMarker,
  MapContainer,
  Popup,
  TileLayer,
  useMap,
} from "react-leaflet";
import { getStopsForTeam } from "../../shared/api/client";
import type { Stop } from "../../shared/api/models/stop";

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
  const params = useParams();
  const campaignId = params.campaignId!;
  const teamId = params.teamId!;

  const [stops, setStops] = useState<Stop[]>([]);

  useEffect(() => {
    if (campaignId) {
      getStopsForTeam(campaignId, teamId).then(setStops);
    }
  }, [campaignId, teamId]);

  const visibleStops = stops.filter((s) => s.stopType !== "Delivered");

  return (
    <div className="h-[calc(100vh-4rem)] w-full">
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
            <Popup>
              <div className="font-semibold">{stop.address.displayName}</div>
              <div>{stop.amount} træer</div>
              <div>{stop.stopType}</div>
            </Popup>
          </CircleMarker>
        ))}
      </MapContainer>
    </div>
  );
}
