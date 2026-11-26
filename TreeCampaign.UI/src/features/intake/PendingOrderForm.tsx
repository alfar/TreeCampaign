import { useState } from "react";
import { deleteOrder } from "../../shared/api/client";
import type { Order } from "../../shared/api/models/order";

interface PendingOrderFormProps {
  order: Pick<Order, "id" | "message">;
  campaignId: string;
  onDeleted: () => void;
}

export default function PendingOrderForm({ order, campaignId, onDeleted }: PendingOrderFormProps) {
  const [isDeleting, setIsDeleting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleDelete = async () => {
    setIsDeleting(true);
    setError(null);
    try {
      const res = await deleteOrder(campaignId, order.id);
      if (res.ok) {
        onDeleted();
      } else {
        setError("Noget gik galt. Prøv igen.");
      }
    } finally {
      setIsDeleting(false);
    }
  };

  return (
    <div className="flex flex-col gap-4">
      <div>
        <p className="text-xs text-gray-500 mb-1">Betalingskode</p>
        <p className="text-sm text-gray-800 bg-gray-50 p-3 rounded border">{order.message}</p>
      </div>

      <p className="text-sm text-gray-600">
        Bestillingen flyttes til fanen "Papirkurv", hvor den kan gendannes igen.
      </p>

      {error && <p className="text-sm text-red-600">{error}</p>}

      <button
        type="button"
        onClick={handleDelete}
        disabled={isDeleting}
        className="self-start bg-red-600 text-white py-2 px-5 rounded disabled:opacity-40"
      >
        {isDeleting ? "Sletter…" : "Slet bestilling"}
      </button>
    </div>
  );
}
