import { useParams } from "react-router-dom";
import {
  collectStop,
  correctStop,
  deliverLoad,
  getStopsForTeam,
  getStreetsByZipCode,
  markStopUnresolved,
  reportTrailerFull,
  requestPickup,
  retryStop,
} from "../../shared/api/client";
import { useEffect, useState } from "react";
import type { Stop } from "../../shared/api/models/stop";
import type { Street } from "../../shared/api/models/street";

export default function TeamStopsTab() {
  const params = useParams();
  const campaignId = params.campaignId!;
  const teamId = params.teamId!;

  const [stops, setStops] = useState<Stop[]>([]);
  const [activeStop, setActiveStop] = useState<string | null>(null);
  const [showPickupForm, setShowPickupForm] = useState(false);

  useEffect(() => {
    if (campaignId) {
      getStopsForTeam(campaignId, teamId).then(setStops);
    }
  }, [campaignId, teamId]);

  function updateStop(stop: Stop) {
    setStops((prevStops) =>
      prevStops.map((s) => (s.id === stop.id ? stop : s)),
    );
  }

  function getStopButtons(stop: Stop) {
    if (activeStop === stop.id) {
      if (stop.stopType === "Assigned") {
        return (
          <div className="flex gap-2 mt-4">
            <button
              className="flex-1 bg-green-600 text-white py-3 rounded-xl"
              onClick={() =>
                collectStop(campaignId, stop.id).then((newStop) =>
                  updateStop(newStop),
                )
              }
            >
              Hentet
            </button>
            <button
              className="flex-1 bg-red-600 text-white py-3 rounded-xl"
              onClick={() =>
                markStopUnresolved(campaignId, stop.id).then((newStop) =>
                  updateStop(newStop),
                )
              }
            >
              Ikke fundet
            </button>
          </div>
        );
      } else if (stop.stopType === "Unresolved") {
        return (
          <div className="flex gap-2 mt-4">
            <button
              className="flex-1 bg-green-600 text-white py-3 rounded-xl"
              onClick={() =>
                retryStop(campaignId, stop.id).then((newStop) =>
                  updateStop(newStop),
                )
              }
            >
              Genoptag
            </button>
          </div>
        );
      } else if (stop.stopType === "Collected") {
        return (
          <div className="flex gap-2 mt-4">
            <button
              className="flex-1 bg-red-600 text-white py-3 rounded-xl"
              onClick={() =>
                correctStop(campaignId, stop.id).then((newStop) =>
                  updateStop(newStop),
                )
              }
            >
              Fortryd
            </button>
          </div>
        );
      }
    }
    return null;
  }

  const visibleStops = stops.filter((s) => s.stopType !== "Delivered");
  const hasCollected = stops.some((s) => s.stopType === "Collected");

  return (
    <div className="m-4 flex flex-col gap-4">
      <div className="flex gap-2">
        <button
          className="flex-1 bg-orange-500 text-white py-3 rounded-xl font-medium"
          onClick={() => reportTrailerFull(campaignId, teamId)}
        >
          Trailer fuld
        </button>
        {hasCollected && (
          <button
            className="flex-1 bg-green-700 text-white py-3 rounded-xl font-medium"
            onClick={() =>
              deliverLoad(campaignId, teamId).then(() =>
                getStopsForTeam(campaignId, teamId).then(setStops),
              )
            }
          >
            Lever last
          </button>
        )}
      </div>

      <button
        className="w-full bg-blue-600 text-white py-3 rounded-xl font-medium"
        onClick={() => setShowPickupForm((v) => !v)}
      >
        {showPickupForm ? "Annuller afhentning" : "Anmod om afhentning"}
      </button>

      {showPickupForm && (
        <PickupForm
          campaignId={campaignId}
          onCreated={(stop) => {
            setStops((prev) => [...prev, stop]);
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
              {getStopButtons(stop)}
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
              {getStopButtons(stop)}
            </li>
          ))}
      </ol>
    </div>
  );
}

function PickupForm({
  campaignId,
  onCreated,
}: {
  campaignId: string;
  onCreated: (stop: Stop) => void;
}) {
  const [streets, setStreets] = useState<Street[]>([]);
  const [streetSearch, setStreetSearch] = useState("");
  const [selectedStreet, setSelectedStreet] = useState<Street | null>(null);
  const [houseNumber, setHouseNumber] = useState("");
  const [treeCount, setTreeCount] = useState(1);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (streetSearch.length >= 2) {
      getStreetsByZipCode("8600").then(setStreets);
    } else {
      setStreets([]);
    }
  }, [streetSearch]);

  const filteredStreets = streets.filter((s) =>
    s.name.toLowerCase().includes(streetSearch.toLowerCase()),
  );

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!selectedStreet || !houseNumber) return;
    setSubmitting(true);
    setError(null);
    try {
      const stop = await requestPickup(
        campaignId,
        selectedStreet.id,
        houseNumber,
        treeCount,
      );
      onCreated(stop);
    } catch {
      setError("Adressen kunne ikke valideres. Prøv igen.");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="border rounded-xl p-4 flex flex-col gap-3 bg-gray-50"
    >
      <h3 className="font-semibold text-base">Anmod om afhentning</h3>

      <div className="relative">
        <input
          type="text"
          placeholder="Søg gade..."
          value={selectedStreet ? selectedStreet.name : streetSearch}
          onChange={(e) => {
            setStreetSearch(e.target.value);
            setSelectedStreet(null);
          }}
          className="w-full border rounded-lg px-3 py-2"
        />
        {filteredStreets.length > 0 && !selectedStreet && (
          <ul className="absolute z-10 w-full bg-white border rounded-lg mt-1 max-h-48 overflow-y-auto shadow">
            {filteredStreets.map((s) => (
              <li
                key={s.id}
                className="px-3 py-2 cursor-pointer hover:bg-blue-50"
                onClick={() => {
                  setSelectedStreet(s);
                  setStreetSearch(s.name);
                  setStreets([]);
                }}
              >
                {s.name}
              </li>
            ))}
          </ul>
        )}
      </div>

      <input
        type="text"
        placeholder="Husnummer (fx 12A)"
        value={houseNumber}
        onChange={(e) => setHouseNumber(e.target.value)}
        className="border rounded-lg px-3 py-2"
      />

      <div className="flex items-center gap-3">
        <label className="text-sm">Antal træer</label>
        <input
          type="number"
          min={1}
          value={treeCount}
          onChange={(e) => setTreeCount(Number(e.target.value))}
          className="border rounded-lg px-3 py-2 w-20"
        />
      </div>

      {error && <p className="text-red-600 text-sm">{error}</p>}

      <button
        type="submit"
        disabled={!selectedStreet || !houseNumber || submitting}
        className="bg-blue-600 text-white py-2 rounded-xl disabled:opacity-50"
      >
        {submitting ? "Sender..." : "Send afhentningsanmodning"}
      </button>
    </form>
  );
}
