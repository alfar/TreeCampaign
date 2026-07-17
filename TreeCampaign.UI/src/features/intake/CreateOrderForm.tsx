import { useState } from "react";
import { createOrder } from "../../shared/api/client";
import { AddressPicker, type Address } from "../../shared/components/AddressPicker";

interface CreateOrderFormProps {
  campaignId: string;
  defaultZipCode: string;
  onOrderCreated: () => void;
}

function todayIso() {
  return new Date().toISOString().split("T")[0];
}

export default function CreateOrderForm({ campaignId, defaultZipCode, onOrderCreated }: CreateOrderFormProps) {
  const [senderName, setSenderName] = useState("");
  const [senderPhoneNumber, setSenderPhoneNumber] = useState("");
  const [amount, setAmount] = useState("40");
  const [orderDate, setOrderDate] = useState(todayIso);
  const [address, setAddress] = useState<Address>({ zipCode: defaultZipCode, street: null, streetName: "", houseNumber: "", isValid: null });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const message = `${address.streetName.trim()} ${address.houseNumber.trim()}, ${address.zipCode.trim()}`;
  const canSubmit =
    senderName.trim().length > 0 &&
    Number(amount) > 0 &&
    orderDate.length > 0 &&
    address.streetName.trim().length > 0 &&
    address.houseNumber.trim().length > 0 &&
    address.zipCode.trim().length > 0 &&
    !isSubmitting;

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (!canSubmit) return;

    setIsSubmitting(true);
    setError(null);
    try {
      const res = await createOrder(campaignId, {
        orderDate: new Date(orderDate).toISOString(),
        senderName: senderName.trim(),
        senderPhoneNumber: senderPhoneNumber.trim() || undefined,
        amount: Number(amount),
        message,
      });
      if (res.ok) {
        onOrderCreated();
      } else {
        setError("Noget gik galt. Prøv igen.");
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4">
      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="text-sm font-medium text-gray-700 block mb-1">Navn</label>
          <input
            type="text"
            value={senderName}
            onChange={(e) => setSenderName(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
            placeholder="Anders Hansen"
          />
        </div>
        <div>
          <label className="text-sm font-medium text-gray-700 block mb-1">Telefon (valgfri)</label>
          <input
            type="tel"
            value={senderPhoneNumber}
            onChange={(e) => setSenderPhoneNumber(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
            placeholder="12345678"
          />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="text-sm font-medium text-gray-700 block mb-1">Beløb (kr.)</label>
          <input
            type="number"
            min="1"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
          />
        </div>
        <div>
          <label className="text-sm font-medium text-gray-700 block mb-1">Dato</label>
          <input
            type="date"
            value={orderDate}
            onChange={(e) => setOrderDate(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
          />
        </div>
      </div>

      <div className="border-t pt-4 flex flex-col gap-3">
        <AddressPicker defaultZipCode={defaultZipCode} onChange={setAddress} />

        {address.streetName.trim() && address.houseNumber.trim() && address.zipCode.trim() && (
          <div>
            <p className="text-xs text-gray-500 mb-1">Besked der gemmes</p>
            <p className="text-sm font-mono bg-gray-50 border rounded px-3 py-2">{message}</p>
          </div>
        )}
      </div>

      {error && <p className="text-sm text-red-600">{error}</p>}

      <button
        type="submit"
        disabled={!canSubmit}
        className="self-start bg-blue-600 text-white py-2 px-5 rounded disabled:opacity-40"
      >
        {isSubmitting ? "Opretter…" : "Opret bestilling"}
      </button>
    </form>
  );
}
