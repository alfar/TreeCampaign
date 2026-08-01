import { useState } from "react";
import { settleOrder, undoTransferOrder } from "../../shared/api/client";
import type { Order } from "../../shared/api/models/order";

interface UndoTransferFormProps {
  order: Pick<Order, "id" | "message">;
  campaignId: string;
  territoryName: string | undefined;
  onUndone: () => void;
  onSettled: () => void;
}

export default function UndoTransferForm({ order, campaignId, territoryName, onUndone, onSettled }: UndoTransferFormProps) {
  const [pendingAction, setPendingAction] = useState<"undo" | "settle" | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleUndo = async () => {
    setPendingAction("undo");
    setError(null);
    try {
      const res = await undoTransferOrder(campaignId, order.id);
      if (res.ok) {
        onUndone();
      } else {
        setError("Noget gik galt. Prøv igen.");
      }
    } finally {
      setPendingAction(null);
    }
  };

  const handleSettle = async () => {
    setPendingAction("settle");
    setError(null);
    try {
      const res = await settleOrder(campaignId, order.id);
      if (res.ok) {
        onSettled();
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

      <p className="text-sm text-gray-800">
        Overført til: <span className="font-medium">{territoryName ?? "ukendt område"}</span>
      </p>

      {error && <p className="text-sm text-red-600">{error}</p>}

      <div className="flex gap-2">
        <button
          type="button"
          onClick={handleSettle}
          disabled={pendingAction !== null}
          className="self-start bg-green-600 text-white py-2 px-5 rounded disabled:opacity-40"
        >
          {pendingAction === "settle" ? "Markerer…" : "Marker som betalt"}
        </button>
        <button
          type="button"
          onClick={handleUndo}
          disabled={pendingAction !== null}
          className="self-start bg-gray-100 text-gray-700 py-2 px-5 rounded border hover:bg-gray-200 disabled:opacity-40"
        >
          {pendingAction === "undo" ? "Fortryder…" : "Fortryd overførsel"}
        </button>
      </div>
    </div>
  );
}
