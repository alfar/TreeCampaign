import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  CircleMarker,
  MapContainer,
  Popup,
  TileLayer,
  useMap,
} from "react-leaflet";
import { getStops } from "../../shared/api/client";
import type { Stop } from "../../shared/api/models/stop";
import NavigationPage from "../../shared/components/NavigationPage";
import ProgressBar from "../../components/ProgressBar";

const SILKEBORG_CENTER: [number, number] = [56.1697, 9.5451];

const STOP_COLORS: Record<string, string> = {
  Unassigned: "#2563eb",
  Assigned: "#2563eb",
  Collected: "#16a34a",
  Delivered: "#16a34a",
  Unresolved: "#dc2626",
};

function MapResizeHandler() {
  const map = useMap();

  useEffect(() => {
    const container = map.getContainer();
    const observer = new ResizeObserver(() => map.invalidateSize());
    observer.observe(container);
    return () => observer.disconnect();
  }, [map]);

  return null;
}

function FitBoundsToStops({ stops }: { stops: Stop[] }) {
  const map = useMap();
  const [hasFitted, setHasFitted] = useState(false);

  useEffect(() => {
    if (hasFitted || stops.length === 0) return;
    const bounds: [number, number][] = stops.map((s) => [
      s.address.latitude,
      s.address.longitude,
    ]);
    map.fitBounds(bounds, { padding: [32, 32] });
    setHasFitted(true);
  }, [stops, map, hasFitted]);

  return null;
}

export default function OverviewMapScreen() {
  const params = useParams();
  const campaignId = params.campaignId!;

  const [stops, setStops] = useState<Stop[]>([]);

  useEffect(() => {
    if (campaignId) {
      getStops(campaignId).then(setStops);
    }
  }, [campaignId]);

  useEffect(() => {
    if (!campaignId) return;

    const es = new EventSource(`/api/${campaignId}/events`);

    es.addEventListener("campaign-update", (e: MessageEvent) => {
      const { type, data } = JSON.parse(e.data) as { type: string; data: Record<string, unknown> };

      const patchStopFunc = (stopId: string, patch: Partial<Stop>) => {
        return () => {
          setStops((prev) =>
            prev.map((s) => (s.id === stopId ? { ...s, ...patch } : s)),
          );
        };
      };

      const actionByEvent: Record<string, () => void> = {
        StopCreated: () => {
          setStops((prev) => [...prev, {
            id: data.id as string,
            address: data.address as Stop["address"],
            amount: data.amount as number,
            stopType: "Unassigned",
            assignedTeamId: undefined,
          }]);
        },
        StopAssigned: patchStopFunc(data.id as string, {
          stopType: "Assigned",
          assignedTeamId: data.assignedTeamId as string,
        }),
        StopUnassigned: patchStopFunc(data.id as string, {
          stopType: "Unassigned",
          assignedTeamId: undefined,
        }),
        StopCollected: patchStopFunc(data.id as string, { stopType: "Collected" }),
        StopCollectionCorrected: patchStopFunc(data.id as string, { stopType: "Assigned" }),
        StopDelivered: patchStopFunc(data.id as string, { stopType: "Delivered" }),
        StopMarkedUnresolved: patchStopFunc(data.id as string, { stopType: "Unresolved" }),
        StopReassigned: patchStopFunc(data.id as string, {
          stopType: "Assigned",
          assignedTeamId: data.assignedTeamId as string,
        }),
        StopReopened: patchStopFunc(data.id as string, {
          stopType: "Unassigned",
          assignedTeamId: undefined,
        }),
        StopRetried: patchStopFunc(data.id as string, { stopType: "Assigned" }),
      };

      const action = actionByEvent[type];
      if (action !== undefined) {
        action();
      }
    });

    return () => es.close();
  }, [campaignId]);

  const counts = stops.reduce(
    (acc, stop) => {
      acc.total += stop.amount;
      switch (stop.stopType) {
        case "Unassigned":
          acc.unassigned += stop.amount;
          break;
        case "Assigned":
          acc.pending += stop.amount;
          break;
        case "Unresolved":
          acc.unresolved += stop.amount;
          break;
        case "Collected":
        case "Delivered":
          acc.collected += stop.amount;
          break;
      }
      return acc;
    },
    { unassigned: 0,pending: 0, unresolved: 0, collected: 0, total: 0 },
  );

  return (
    <NavigationPage>
      <div className="h-[calc(100vh-2rem)] w-full flex flex-col gap-2">
        <ProgressBar
          parts={[
            { title: "Opsamlet", amount: counts.collected, color: "#16a34a" },
            { title: "Fejlet", amount: counts.unresolved, color: "#dc2626" },
            { title: "Tildelt", amount: counts.pending, color: "#2563eb" },
            { title: "Mangler", amount: counts.unassigned, color: "#ffffff" },
          ]}
          total={counts.total}
        />
        <div className="flex-1">
          <MapContainer
            center={SILKEBORG_CENTER}
            zoom={13}
            className="h-full w-full"
          >
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            <MapResizeHandler />
            <FitBoundsToStops stops={stops} />
            {stops.map((stop) => (
              <CircleMarker
                key={stop.id}
                center={[stop.address.latitude, stop.address.longitude]}
                radius={5}
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
      </div>
    </NavigationPage>
  );
}
