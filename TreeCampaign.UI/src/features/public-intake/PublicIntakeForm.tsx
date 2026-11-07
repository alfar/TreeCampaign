import { useCallback, useEffect, useMemo, useRef, useState } from "react";
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
import { DEFAULT_PRICE_PER_TREE } from "../../shared/campaignPricing";
import {
  AddressPicker,
  type Address,
  type AddressPickerHandle,
} from "../../shared/components/AddressPicker";
import {
  CheckIcon,
  MinusCircleIcon,
  MinusIcon,
  PlusIcon,
} from "@heroicons/react/24/solid";

type Step = 1 | 2 | 3;

const STEPS: { id: Step; label: string }[] = [
  { id: 1, label: "Adresse" },
  { id: 2, label: "Antal" },
  { id: 3, label: "Betaling" },
];

function Stepper({
  current,
  onSelect,
}: {
  current: Step;
  onSelect: (step: Step) => void;
}) {
  return (
    <ol className="mb-6 flex items-start" aria-label="Fremgang">
      {STEPS.map((step, index) => {
        const isDone = step.id < current;
        const isCurrent = step.id === current;
        return (
          <li
            key={step.id}
            className="relative flex flex-1 flex-col items-center"
            aria-current={isCurrent ? "step" : undefined}
          >
            {index > 0 && (
              <span
                aria-hidden
                className={`absolute right-1/2 top-4 h-0.5 w-full -translate-y-1/2 ${
                  step.id <= current ? "bg-blue-700" : "bg-gray-200"
                }`}
              />
            )}
            <button
              type="button"
              disabled={!isDone}
              onClick={() => onSelect(step.id)}
              className={`relative z-10 flex h-8 w-8 items-center justify-center rounded-full text-sm font-semibold disabled:cursor-default ${
                isDone
                  ? "bg-blue-700 text-white"
                  : isCurrent
                    ? "border-2 border-blue-700 bg-white text-blue-700"
                    : "border-2 border-gray-300 bg-white text-gray-400"
              }`}
              aria-label={`Trin ${step.id}: ${step.label}`}
            >
              {isDone ? <CheckIcon className="h-4 w-4" /> : step.id}
            </button>
            <span
              className={`mt-1 text-xs ${
                isCurrent ? "font-semibold text-blue-700" : "text-gray-500"
              }`}
            >
              {step.label}
            </span>
          </li>
        );
      })}
    </ol>
  );
}

function SummaryBlock({
  values,
}: {
  values: {
    street: string;
    houseNumber: string;
    name?: string;
    phoneNumber?: string;
    numberOfTrees?: number;
  };
}) {
  return (
    <div className="mb-4 rounded-md border border-blue-200 bg-blue-50 px-3 py-2 text-blue-700">
      <p className="mt-1 text-gray-700">
        <span className="font-bold">Adresse:</span> {values.street}{" "}
        {values.houseNumber}
      </p>
      {values.name && (
        <p className="mt-1 text-gray-700">
          <span className="font-bold">Navn:</span> {values.name}
        </p>
      )}
      {values.phoneNumber && (
        <p className="mt-1 text-gray-700">
          <span className="font-bold">Telefon:</span> {values.phoneNumber}
        </p>
      )}
      {values.numberOfTrees !== undefined && (
        <p className="mt-1 text-gray-700">
          <span className="font-bold">Antal træer:</span> {values.numberOfTrees}
        </p>
      )}
    </div>
  );
}

const inputClass =
  "w-full rounded-lg border border-gray-300 px-3 py-2 text-base";

export default function PublicIntakeForm({
  campaignId,
}: {
  campaignId: string;
}) {
  const [step, setStep] = useState<Step>(1);
  const addressPickerRef = useRef<AddressPickerHandle>(null);
  const [territoryId, setTerritoryId] = useState<string | null>(null);
  const [streets, setStreets] = useState<Street[]>([]);
  const [streetName, setStreetName] = useState("");
  const [isStreetSelected, setIsStreetSelected] = useState(false);
  const [houseNumber, setHouseNumber] = useState("");
  const [isLoadingTerritory, setIsLoadingTerritory] = useState(true);
  const [isLoadingStreets, setIsLoadingStreets] = useState(false);
  const [isValidatingAddress, setIsValidatingAddress] = useState(false);
  const [validationError, setValidationError] = useState<string | null>(null);
  const [resolvedAddress, setResolvedAddress] =
    useState<ResolvedStreetSectionAddress | null>(null);

  const [treeCount, setTreeCount] = useState(1);
  const [pricePerTree, setPricePerTree] = useState(DEFAULT_PRICE_PER_TREE);
  const [senderName, setSenderName] = useState("");
  const [senderPhoneNumber, setSenderPhoneNumber] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [orderResult, setOrderResult] = useState<PendingOrderResult | null>(
    null,
  );
  const [paymentCode, setPaymentCode] = useState<string | null>(null);
  const [submittedKey, setSubmittedKey] = useState<string | null>(null);

  const selectedStreet = useMemo(
    () =>
      streets.find(
        (street) =>
          street.name.toLowerCase() === streetName.trim().toLowerCase(),
      ) ?? null,
    [streetName, streets],
  );
  const selectedStreetId = selectedStreet?.id ?? "";
  const defaultZipCode = streets[0]?.zipCode ?? "8600";
  // Only warn once a street has been picked; while typing, no match yet is expected.
  const streetNotInTerritory =
    isStreetSelected && streetName.trim().length > 0 && !selectedStreet;

  const totalAmount = treeCount * pricePerTree;

  useEffect(() => {
    let cancelled = false;

    async function loadCampaignSummary() {
      try {
        const summary: CampaignSummary = await getCampaignSummary(campaignId);
        if (cancelled) return;
        setTerritoryId(summary.territoryId ?? null);
        setPricePerTree(summary.pricePerTree ?? DEFAULT_PRICE_PER_TREE);
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

  const handleAddressChange = useCallback((address: Address) => {
    setStreetName(address.streetName);
    setIsStreetSelected(address.isStreetSelected);
    setHouseNumber(address.houseNumber);
  }, []);

  // A changed address invalidates the resolved address and any order made for the old one.
  useEffect(() => {
    setResolvedAddress(null);
    setOrderResult(null);
    setPaymentCode(null);
    setSubmittedKey(null);
  }, [selectedStreetId, houseNumber]);

  const goToStep = (target: Step) => {
    setValidationError(null);
    setStep(target);
  };

  const handleValidateAddress = async () => {
    if (resolvedAddress) {
      goToStep(2);
      return;
    }

    if (!territoryId || !selectedStreetId || !houseNumber.trim()) {
      setValidationError("Vælg en gade og skriv et husnummer.");
      return;
    }

    setIsValidatingAddress(true);
    setValidationError(null);
    setResolvedAddress(null);

    try {
      const data = await resolveStreetSectionForAddress(
        territoryId,
        selectedStreetId,
        houseNumber.trim(),
      );
      setResolvedAddress(data);
      setStep(2);
    } catch {
      setValidationError("Vi henter desværre ikke juletræer på din adresse.");
    } finally {
      setIsValidatingAddress(false);
    }
  };

  const orderKey = `${treeCount}|${senderName.trim()}|${senderPhoneNumber.trim()}`;

  const handleSubmitOrder = async () => {
    if (!resolvedAddress) {
      setValidationError("Check først adressen før du fortsætter.");
      return;
    }

    // Going back and forth without changing anything must not create a duplicate order.
    if (orderResult && submittedKey === orderKey) {
      goToStep(3);
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
      setSubmittedKey(orderKey);
      setStep(3);
    } catch {
      setValidationError("Bestillingen kunne ikke oprettes.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCompleteOrder = () => {
    // Reset the form to allow a new order.
    setStep(1);
    setStreetName("");
    setHouseNumber("");
    setResolvedAddress(null);
    setOrderResult(null);
    setPaymentCode(null);
    setSubmittedKey(null);
    setTreeCount(1);
    setSenderName("");
    setSenderPhoneNumber("");
    addressPickerRef.current?.reset();
  };

  const canValidateAddress =
    Boolean(territoryId && selectedStreetId && houseNumber.trim()) &&
    !isValidatingAddress;
  const amountDueLabel = `${totalAmount} kr.`;
  const mobilePayImageUrl = "/mobilepay.png";

  const backButton = (target: Step) => (
    <button
      type="button"
      onClick={() => goToStep(target)}
      className="w-full rounded-lg border border-gray-300 bg-white px-4 py-3 text-base font-medium text-gray-700 sm:w-auto"
    >
      Tilbage
    </button>
  );

  return (
    <div className="mx-auto max-w-3xl rounded-2xl border border-gray-200 bg-white shadow-sm">
      <h1 className="flex items-center justify-center rounded-t-2xl bg-blue-800 text-xl font-bold text-white sm:text-3xl">
        <img
          src="/logo.png"
          alt="Logo"
          className="mr-2 h-16 w-16 rounded-full p-3 sm:h-24 sm:w-24 sm:p-4"
        />{" "}
        1. Silkeborg Gruppe
      </h1>
      <div className="p-4 sm:p-6">
        <h2 className="mb-5 text-xl font-bold text-gray-900 sm:text-2xl">
          Bestil afhentning af juletræer
        </h2>

        {isLoadingTerritory ? (
          <p className="text-sm text-gray-600">Henter område…</p>
        ) : !territoryId ? (
          <p className="text-sm text-red-600">
            Dette kampagneområde er ikke tilgængeligt.
          </p>
        ) : (
          <div>
            <Stepper current={step} onSelect={goToStep} />

            {validationError && (
              <p className="mb-4 rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
                {validationError}
              </p>
            )}

            {/* Kept mounted (just hidden) so AddressPicker keeps its state when navigating back. */}
            <div className="space-y-3" hidden={step !== 1}>
              {isLoadingStreets ? (
                <p className="text-sm text-gray-500">Henter gader…</p>
              ) : (
                <AddressPicker
                  ref={addressPickerRef}
                  defaultZipCode={defaultZipCode}
                  onChange={handleAddressChange}
                />
              )}
              {streetNotInTerritory && (
                <h2 className="flex items-center gap-2 text-xl font-bold text-gray-900">
                  <MinusCircleIcon className="h-6 w-6 shrink-0 text-red-500" />
                  Vi henter desværre ikke juletræer på denne vej.
                </h2>
              )}
              <button
                type="button"
                disabled={!canValidateAddress}
                onClick={handleValidateAddress}
                className="w-full rounded-lg bg-blue-700 px-4 py-3 text-base font-medium text-white disabled:cursor-not-allowed disabled:bg-blue-300 sm:w-auto"
              >
                {isValidatingAddress ? "Checker…" : "Check adresse"}
              </button>
            </div>

            {step === 2 && resolvedAddress && (
              <div>
                <SummaryBlock
                  values={{
                    street: selectedStreet?.name ?? "",
                    houseNumber: resolvedAddress.houseNumber,
                  }}
                />

                <h2 className="flex items-center gap-2 text-xl font-bold text-gray-900">
                  <CheckIcon className="h-6 w-6 shrink-0 text-green-500" /> Vi
                  vil gerne hente {treeCount == 1 ? "dit træ" : "dine træer"}
                </h2>

                <div className="mt-5">
                  <div>
                    <label className="mb-1 block text-sm font-medium text-gray-700">
                      Antal træer
                    </label>
                    <div className="flex items-stretch gap-2">
                      <button
                        type="button"
                        aria-label="Færre træer"
                        disabled={treeCount <= 1}
                        onClick={() =>
                          setTreeCount((count) => Math.max(1, count - 1))
                        }
                        className="flex h-14 w-14 shrink-0 touch-manipulation select-none items-center justify-center rounded-lg border border-gray-300 bg-white text-3xl font-bold text-gray-700 active:bg-gray-100 disabled:opacity-40"
                      >
                        <MinusIcon className="h-6 w-6" />
                      </button>
                      <input
                        type="number"
                        inputMode="numeric"
                        min={1}
                        value={treeCount}
                        onChange={(event) =>
                          setTreeCount(
                            Math.max(1, Number(event.target.value || 1)),
                          )
                        }
                        className={
                          inputClass +
                          " h-14 min-w-0 flex-1 text-center text-xl"
                        }
                      />
                      <button
                        type="button"
                        aria-label="Flere træer"
                        onClick={() => setTreeCount((count) => count + 1)}
                        className="flex h-14 w-14 shrink-0 touch-manipulation select-none items-center justify-center rounded-lg border border-gray-300 bg-white text-3xl font-bold text-gray-700 active:bg-gray-100"
                      >
                        <PlusIcon className="h-6 w-6" />
                      </button>
                    </div>
                  </div>

                  <div className="mt-2">
                    <label className="mb-1 block text-lg font-medium text-gray-700 text-center">
                      Beløb: {amountDueLabel}
                    </label>
                  </div>

                  <div>
                    <label className="mb-1 block text-sm font-medium text-gray-700">
                      Navn (valgfrit)
                    </label>
                    <input
                      type="text"
                      autoComplete="name"
                      value={senderName}
                      onChange={(event) => setSenderName(event.target.value)}
                      className={inputClass}
                      placeholder="F.eks. Anna Jensen"
                    />
                  </div>

                  <div>
                    <label className="mb-1 block text-sm font-medium text-gray-700">
                      Telefon (valgfrit)
                    </label>
                    <input
                      type="tel"
                      autoComplete="tel"
                      value={senderPhoneNumber}
                      onChange={(event) =>
                        setSenderPhoneNumber(event.target.value)
                      }
                      className={inputClass}
                      placeholder="12345678"
                    />
                  </div>
                </div>

                <div className="mt-6 flex flex-col-reverse gap-3 sm:flex-row">
                  {backButton(1)}
                  <button
                    type="button"
                    onClick={handleSubmitOrder}
                    disabled={isSubmitting}
                    className="w-full rounded-lg bg-green-700 px-4 py-3 text-base font-medium text-white disabled:cursor-not-allowed disabled:bg-green-300 sm:w-auto"
                  >
                    {isSubmitting ? "Opretter bestilling…" : "Opret bestilling"}
                  </button>
                </div>
              </div>
            )}

            {step === 3 && orderResult && paymentCode && (
              <div>
                <SummaryBlock
                  values={{
                    street: selectedStreet?.name ?? "",
                    houseNumber: resolvedAddress?.houseNumber ?? "",
                    name: senderName,
                    phoneNumber: senderPhoneNumber,
                    numberOfTrees: treeCount,
                  }}
                />

                <h2 className="flex items-center gap-2 text-xl font-bold text-gray-900">
                  <CheckIcon className="h-6 w-6 shrink-0 text-green-500" />{" "}
                  Bestillingen er oprettet
                </h2>

                <div className="mt-6 flex flex-col items-center">
                  <p className="mt-2 text-lg text-gray-900">
                    Betal {amountDueLabel} med MobilePay. Skriv koden{" "}
                    <span className="font-mono text-xl font-bold">
                      {paymentCode}
                    </span>{" "}
                    i betalingsbeskeden.
                  </p>
                  <p className="mt-2">
                    <a
                      href="https://qr.mobilepay.dk/vp/OJbdGYMcM"
                      target="_blank"
                      rel="noopener noreferrer"
                    >
                      <img
                        src={mobilePayImageUrl}
                        alt="MobilePay QR-kode"
                        className="h-48 w-48 max-w-full"
                      />
                    </a>
                  </p>
                  <p className="mt-2 text-center text-lg text-gray-900">
                    Scan QR-koden med MobilePay - er du på mobilen, kan du
                    trykke på billedet for at åbne MobilePay.
                  </p>
                  <p className="mt-2 text-center text-lg text-gray-900">
                    Tak for din bestilling! Vi glæder os til at hente dine
                    juletræer.
                  </p>
                </div>

                <div className="mt-6 flex flex-col-reverse gap-3 sm:flex-row">
                  {backButton(2)}
                  <button
                    type="button"
                    onClick={handleCompleteOrder}
                    className="w-full rounded-lg bg-green-700 px-4 py-3 text-base font-medium text-white sm:w-auto"
                  >
                    Afslut bestilling
                  </button>
                </div>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
