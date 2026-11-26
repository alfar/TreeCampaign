import { useState } from "react";
import { undeleteOrder } from "../../shared/api/client";
import type { Order } from "../../shared/api/models/order";

interface DeletedOrderFormProps {
  order: Pick<Order, "id" | "message">;
  campaignId: string;
  onUndeleted: () => void;
}

export default function DeletedOrderForm({ order, campaignId, onUndeleted }: DeletedOrderFormProps) {
  const [isRestoring, setIsRestoring] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleUndelete = async () => {
    setIsRestoring(true);
    setError(null);
    try {
      const res = await undeleteOrder(campaignId, order.id);
      if (res.ok) {
        onUndeleted();
      } else {
        setError("Noget gik galt. Prøv igen.");
      }
    } finally {
      setIsRestoring(false);
    }
  };

  return (
    <div className="flex flex-col gap-4">
      <div>
        <p className="text-xs text-gray-500 mb-1">Betalingskode</p>
        <p className="text-sm text-gray-800 bg-gray-50 p-3 rounded border">{order.message}</p>
      </div>

      <p className="text-sm text-gray-600">
        Bestillingen er slettet. Gendan den for at få den tilbage i fanen "Afventer betaling".
      </p>

      {error && <p className="text-sm text-red-600">{error}</p>}

      <button
        type="button"
        onClick={handleUndelete}
        disabled={isRestoring}
        className="self-start bg-gray-100 text-gray-700 py-2 px-5 rounded border hover:bg-gray-200 disabled:opacity-40"
      >
        {isRestoring ? "Gendanner…" : "Gendan bestilling"}
      </button>
    </div>
  );
}
