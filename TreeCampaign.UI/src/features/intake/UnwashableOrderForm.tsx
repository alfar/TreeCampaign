import { useState } from "react";
import { markOrderAsDonation, refundOrder, undoMarkOrderUnwashable } from "../../shared/api/client";
import type { Order } from "../../shared/api/models/order";

interface UnwashableOrderFormProps {
  order: Pick<Order, "id" | "message">;
  campaignId: string;
  onRefunded: () => void;
  onDonated: () => void;
  onUndone: () => void;
}

export default function UnwashableOrderForm({ order, campaignId, onRefunded, onDonated, onUndone }: UnwashableOrderFormProps) {
  const [pendingAction, setPendingAction] = useState<"refund" | "donate" | "undo" | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleRefund = async () => {
    setPendingAction("refund");
    setError(null);
    try {
      const res = await refundOrder(campaignId, order.id);
      if (res.ok) {
        onRefunded();
      } else {
        setError("Noget gik galt. Prøv igen.");
      }
    } finally {
      setPendingAction(null);
    }
  };

  const handleDonate = async () => {
    setPendingAction("donate");
    setError(null);
    try {
      const res = await markOrderAsDonation(campaignId, order.id);
      if (res.ok) {
        onDonated();
      } else {
        setError("Noget gik galt. Prøv igen.");
      }
    } finally {
      setPendingAction(null);
    }
  };

  const handleUndo = async () => {
    setPendingAction("undo");
    setError(null);
    try {
      const res = await undoMarkOrderUnwashable(campaignId, order.id);
      if (res.ok) {
        onUndone();
      } else {
        setError("Noget gik galt. Prøv igen.");
      }
    } finally {
      setPendingAction(null);
    }
  };

  return (
    <div className="flex flex-col gap-4">
      <div>
        <p className="text-xs text-gray-500 mb-1">Original besked</p>
        <p className="text-sm text-gray-800 bg-gray-50 p-3 rounded border">{order.message}</p>
      </div>

      <p className="text-sm text-gray-600">
        Denne bestilling bliver ikke til et stop. Vælg om beløbet skal tilbagebetales, eller om det beholdes som en donation.
      </p>

      {error && <p className="text-sm text-red-600">{error}</p>}

      <div className="flex gap-2">
        <button
          type="button"
          onClick={handleRefund}
          disabled={pendingAction !== null}
          className="self-start bg-blue-600 text-white py-2 px-5 rounded disabled:opacity-40"
        >
          {pendingAction === "refund" ? "Markerer…" : "Marker som tilbagebetalt"}
        </button>
        <button
          type="button"
          onClick={handleDonate}
          disabled={pendingAction !== null}
          className="self-start bg-green-600 text-white py-2 px-5 rounded disabled:opacity-40"
        >
          {pendingAction === "donate" ? "Markerer…" : "Marker som donation"}
        </button>
        <button
          type="button"
          onClick={handleUndo}
          disabled={pendingAction !== null}
          className="self-start bg-gray-100 text-gray-700 py-2 px-5 rounded border hover:bg-gray-200 disabled:opacity-40"
        >
          {pendingAction === "undo" ? "Fortryder…" : "Fortryd"}
        </button>
      </div>
    </div>
  );
}
