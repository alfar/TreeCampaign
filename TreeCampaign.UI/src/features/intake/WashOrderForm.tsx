import { useMemo, useState } from "react";
import { createStreet, markOrderUnwashable, washOrder } from "../../shared/api/client";
import type { Order } from "../../shared/api/models/order";
import type { Street } from "../../shared/api/models/street";
import { AddressPicker, type Address } from "../../shared/components/AddressPicker";

interface WashOrderFormProps {
  order: Pick<Order, "id" | "message" | "errorMessage">;
  campaignId: string;
  defaultZipCode: string;
  onStreetAdded?: () => void;
  onWashed?: () => void;
  onMarkedUnwashable?: () => void;
}

// Best-effort prefill only — this is exactly the message the backend parser/washer
// already failed to resolve, so the result here is a starting point, not a guarantee.
const MESSAGE_PATTERN = /(?<street>\p{L}[\p{L}0-9\s\-.]+)\s+(?<number>\d+)\s*(?<letter>\p{L})?(?:\s*,\s*(?<zip>\d{4}))?\s*$/u;

function parseMessage(message: string): { streetName: string; houseNumber: string; zipCode: string | null } | null {
  const match = MESSAGE_PATTERN.exec(message);
  if (!match?.groups) return null;

  const streetName = match.groups.street.trim();
  if (!streetName) return null;

  const houseNumber = `${match.groups.number}${match.groups.letter ?? ""}`;
  const zipCode = match.groups.zip ?? null;

  return { streetName, houseNumber, zipCode };
}

export default function WashOrderForm({ order, campaignId, defaultZipCode, onStreetAdded, onWashed, onMarkedUnwashable }: WashOrderFormProps) {
  const parsed = useMemo(() => parseMessage(order.message), [order.message]);

  const [address, setAddress] = useState<Address>({
    zipCode: parsed?.zipCode ?? defaultZipCode,
    street: null,
    streetName: parsed?.streetName ?? "",
    houseNumber: parsed?.houseNumber ?? "",
    isValid: null,
  });
  const [createdStreet, setCreatedStreet] = useState<Street | null>(null);
  const [isAdding, setIsAdding] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [washError, setWashError] = useState<string | null>(null);
  const [isMarkingUnwashable, setIsMarkingUnwashable] = useState(false);

  const handleMarkUnwashable = async () => {
    setIsMarkingUnwashable(true);
    setWashError(null);
    try {
      const res = await markOrderUnwashable(campaignId, order.id);
      if (res.ok) {
        onMarkedUnwashable?.();
      } else {
        setWashError("Noget gik galt. Prøv igen.");
      }
    } finally {
      setIsMarkingUnwashable(false);
    }
  };

  const effectiveStreet = address.street ?? createdStreet;
  const noMatch = address.streetName.trim().length > 0 && address.zipCode.length === 4 && !effectiveStreet;
  const canSubmit = effectiveStreet !== null && address.houseNumber.trim().length > 0 && !isSubmitting;

  const handleAddressChange = (next: Address) => {
    setAddress(next);
    setCreatedStreet(null);
    setWashError(null);
  };

  const handleAddStreet = async () => {
    setIsAdding(true);
    try {
      const newStreet = await createStreet(address.streetName.trim(), address.zipCode);
      setCreatedStreet(newStreet);
      onStreetAdded?.();
    } finally {
      setIsAdding(false);
    }
  };

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (!effectiveStreet || !address.houseNumber.trim()) return;

    setIsSubmitting(true);
    setWashError(null);
    try {
      const res = await washOrder(campaignId, order.id, {
        streetId: effectiveStreet.id,
        houseNumber: address.houseNumber.trim(),
      });
      if (res.ok) {
        onWashed?.();
      } else if (res.status === 422) {
        setWashError("Husnummeret matcher ingen sektion på denne gade.");
      } else {
        setWashError("Noget gik galt. Prøv igen.");
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4">
      <div>
        <p className="text-xs text-gray-500 mb-1">Original besked</p>
        <p className="text-sm text-gray-800 bg-gray-50 p-3 rounded border">{order.message}</p>
      </div>

      {order.errorMessage && (
        <p className="text-sm text-red-600 bg-red-50 p-3 rounded border border-red-200">
          {order.errorMessage}
        </p>
      )}

      <AddressPicker
        defaultZipCode={parsed?.zipCode ?? defaultZipCode}
        defaultStreetName={parsed?.streetName ?? ""}
        defaultHouseNumber={parsed?.houseNumber ?? ""}
        onChange={handleAddressChange}
      />

      {noMatch && (
        <button
          type="button"
          onClick={handleAddStreet}
          disabled={isAdding}
          className="self-start text-sm text-blue-600 hover:underline disabled:opacity-50"
        >
          {isAdding ? "Tilføjer…" : `+ Tilføj "${address.streetName.trim()}" som ny gade`}
        </button>
      )}

      {washError && (
        <p className="text-sm text-red-600">{washError}</p>
      )}

      <div className="flex items-center gap-4">
        <button
          type="submit"
          disabled={!canSubmit}
          className="self-start bg-blue-600 text-white py-2 px-5 rounded disabled:opacity-40"
        >
          {isSubmitting ? "Gemmer…" : "Gem adresse"}
        </button>
        <button
          type="button"
          onClick={handleMarkUnwashable}
          disabled={isMarkingUnwashable}
          className="text-sm text-gray-500 hover:underline disabled:opacity-50"
        >
          {isMarkingUnwashable ? "Markerer…" : "Kan ikke behandles (tom/ugyldig besked)"}
        </button>
      </div>
    </form>
  );
}
