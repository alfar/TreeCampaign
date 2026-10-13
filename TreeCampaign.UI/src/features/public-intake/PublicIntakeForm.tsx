import { useEffect, useMemo, useState } from "react";
import {
  createPendingOrder,
  getCampaignSummary,
  getTerritoryStreets,
  resolveStreetSectionForAddress,
  type CampaignSummary,
  type PendingOrderResult,
  type ResolvedStreetSectionAddress,
} from "../../shared/api/client";
import type { Street } from "../../shared/api/models/street";

export default function PublicIntakeForm({
  campaignId,
}: {
  campaignId: string;
}) {
  const [territoryId, setTerritoryId] = useState<string | null>(null);
  const [streets, setStreets] = useState<Street[]>([]);
  const [selectedStreetId, setSelectedStreetId] = useState<string>("");
  const [houseNumber, setHouseNumber] = useState("");
  const [isLoadingTerritory, setIsLoadingTerritory] = useState(true);
  const [isLoadingStreets, setIsLoadingStreets] = useState(false);
  const [isValidatingAddress, setIsValidatingAddress] = useState(false);
  const [validationError, setValidationError] = useState<string | null>(null);
  const [resolvedAddress, setResolvedAddress] = useState<ResolvedStreetSectionAddress | null>(null);

  const [treeCount, setTreeCount] = useState(1);
  const [senderName, setSenderName] = useState("");
  const [senderPhoneNumber, setSenderPhoneNumber] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [orderResult, setOrderResult] = useState<PendingOrderResult | null>(null);
  const [paymentCode, setPaymentCode] = useState<string | null>(null);

  const selectedStreet = useMemo(
    () => streets.find((street) => street.id === selectedStreetId) ?? null,
    [selectedStreetId, streets],
  );

  const totalAmount = treeCount * 40;

  useEffect(() => {
    let cancelled = false;

    async function loadCampaignSummary() {
      try {
        const summary: CampaignSummary = await getCampaignSummary(campaignId);
        if (cancelled) return;
        setTerritoryId(summary.territoryId ?? null);
      } catch {
        if (!cancelled) {
          setValidationError("Kampagnen kunne ikke indlæses.");
        }
      } finally {
        if (!cancelled) {
          setIsLoadingTerritory(false);
        }
      }
    }

    loadCampaignSummary();

    return () => {
      cancelled = true;
    };
  }, [campaignId]);

  useEffect(() => {
    if (!territoryId) {
      setStreets([]);
      return;
    }

    const activeTerritoryId = territoryId;
    let cancelled = false;

    async function loadStreets() {
      try {
        setIsLoadingStreets(true);
        const data = await getTerritoryStreets(activeTerritoryId);
        if (!cancelled) {
          setStreets(data);
          if (data.length > 0 && !selectedStreetId) {
            setSelectedStreetId(data[0].id);
          }
        }
      } catch {
        if (!cancelled) {
          setValidationError("Gaderne for området kunne ikke indlæses.");
        }
      } finally {
        if (!cancelled) {
          setIsLoadingStreets(false);
        }
      }
    }

    loadStreets();

    return () => {
      cancelled = true;
    };
  }, [territoryId]);

  const handleValidateAddress = async () => {
    if (!territoryId || !selectedStreetId || !houseNumber.trim()) {
      setValidationError("Vælg en gade og skriv et husnummer.");
      return;
    }

    setIsValidatingAddress(true);
    setValidationError(null);
    setResolvedAddress(null);

    try {
      const data = await resolveStreetSectionForAddress(territoryId, selectedStreetId, houseNumber.trim());
      setResolvedAddress(data);
    } catch {
      setValidationError("Adressen findes ikke i dette område.");
    } finally {
      setIsValidatingAddress(false);
    }
  };

  const handleSubmitOrder = async () => {
    if (!resolvedAddress) {
      setValidationError("Valider først adressen før du fortsætter.");
      return;
    }

    setIsSubmitting(true);
    setValidationError(null);

    try {
      const created = await createPendingOrder(campaignId, {
        senderName: senderName.trim() || "",
        senderPhoneNumber: senderPhoneNumber.trim() || null,
        amount: totalAmount,
        streetId: resolvedAddress.streetId,
        streetSectionId: resolvedAddress.streetSectionId,
        neighborhoodId: resolvedAddress.neighborhoodId,
        houseNumber: resolvedAddress.houseNumber,
      });

      setOrderResult(created);
      setPaymentCode(created.paymentCode);
    } catch {
      setValidationError("Bestillingen kunne ikke oprettes.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const canValidateAddress = Boolean(territoryId && selectedStreetId && houseNumber.trim()) && !isValidatingAddress;
  const amountDueLabel = `${totalAmount} kr.`;
  const mobilePayImageUrl = "/mobilepay.png";

  return (
    <div className="mx-auto max-w-3xl rounded-2xl border border-gray-200 bg-white p-6 shadow-sm">
      <div className="mb-6">
        <p className="text-xs font-semibold uppercase tracking-[0.12em] text-blue-700">Bestil træer</p>
        <h1 className="mt-2 text-2xl font-bold text-gray-900">Bestil din træindsamling</h1>
      </div>

      {isLoadingTerritory ? (
        <p className="text-sm text-gray-600">Henter område…</p>
      ) : !territoryId ? (
        <p className="text-sm text-red-600">Dette kampagneområde er ikke tilgængeligt.</p>
      ) : (
        <div className="space-y-6">
          <div className="grid gap-4 md:grid-cols-2">
            <div className="md:col-span-2">
              <label className="mb-1 block text-sm font-medium text-gray-700">Vælg vej</label>
              {isLoadingStreets ? (
                <p className="text-sm text-gray-500">Henter gader…</p>
              ) : (
                <select
                  value={selectedStreetId}
                  onChange={(event) => setSelectedStreetId(event.target.value)}
                  className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm"
                >
                  {!selectedStreetId && <option value="">Vælg vej</option>}
                  {streets.map((street) => (
                    <option key={street.id} value={street.id}>
                      {street.name}
                    </option>
                  ))}
                </select>
              )}
            </div>

            <div>
              <label className="mb-1 block text-sm font-medium text-gray-700">Husnummer</label>
              <input
                type="text"
                value={houseNumber}
                onChange={(event) => setHouseNumber(event.target.value)}
                placeholder="12A"
                className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm"
              />
            </div>

            <div className="flex items-end">
              <button
                type="button"
                disabled={!canValidateAddress}
                onClick={handleValidateAddress}
                className="w-full rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white disabled:cursor-not-allowed disabled:bg-blue-300"
              >
                {isValidatingAddress ? "Validerer…" : "Valider adresse"}
              </button>
            </div>
          </div>

          {validationError && (
            <p className="rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
              {validationError}
            </p>
          )}

          {resolvedAddress && !orderResult && (
            <div className="rounded-xl border border-green-200 bg-green-50 p-4">
              <p className="text-sm font-medium text-green-900">Adresse bekræftet</p>
              <p className="mt-1 text-sm text-green-800">
                {selectedStreet?.name} {resolvedAddress.houseNumber}
              </p>

              <div className="mt-5 grid gap-4 md:grid-cols-2">
                <div>
                  <label className="mb-1 block text-sm font-medium text-gray-700">Antal træer</label>
                  <input
                    type="number"
                    min={1}
                    value={treeCount}
                    onChange={(event) => setTreeCount(Math.max(1, Number(event.target.value || 1)))}
                    className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm"
                  />
                </div>

                <div>
                  <label className="mb-1 block text-sm font-medium text-gray-700">Beløb</label>
                  <div className="flex h-[42px] items-center rounded-lg border border-gray-300 bg-white px-3 text-sm font-medium text-gray-900">
                    {amountDueLabel}
                  </div>
                </div>
              </div>

              <div className="mt-4 grid gap-4 md:grid-cols-2">
                <div>
                  <label className="mb-1 block text-sm font-medium text-gray-700">Navn (valgfrit)</label>
                  <input
                    type="text"
                    value={senderName}
                    onChange={(event) => setSenderName(event.target.value)}
                    className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm"
                    placeholder="F.eks. Anna Jensen"
                  />
                </div>

                <div>
                  <label className="mb-1 block text-sm font-medium text-gray-700">Telefon (valgfrit)</label>
                  <input
                    type="tel"
                    value={senderPhoneNumber}
                    onChange={(event) => setSenderPhoneNumber(event.target.value)}
                    className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm"
                    placeholder="12345678"
                  />
                </div>
              </div>

              <button
                type="button"
                onClick={handleSubmitOrder}
                disabled={isSubmitting}
                className="mt-5 rounded-lg bg-green-700 px-4 py-2 text-sm font-medium text-white disabled:cursor-not-allowed disabled:bg-green-300"
              >
                {isSubmitting ? "Opretter bestilling…" : "Opret bestilling"}
              </button>
            </div>
          )}

          {orderResult && paymentCode && (
            <div className="rounded-2xl border border-blue-200 bg-blue-50 p-6">
              <h2 className="text-xl font-bold text-gray-900">Bestillingen er oprettet</h2>
              <p className="mt-2 text-sm text-gray-700">
                Betal {amountDueLabel} med MobilePay. Skriv koden <span className="font-mono font-bold">{paymentCode}</span> i betalingsbeskeden.
              </p>

              <div className="mt-6 flex flex-col items-center gap-4 md:flex-row md:items-start md:justify-center">
                <div className="rounded-xl bg-white p-3 shadow-sm">
                  <img src={mobilePayImageUrl} alt="MobilePay QR-kode" className="h-48 w-48" />
                </div>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
