import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getOrders } from "../../shared/api/client";
import type { Order } from "../../shared/api/models/order";
import OrderList from "./OrderList";
import WashOrderForm from "./WashOrderForm";

const DEFAULT_ZIP_CODE = "8600";

export default function IntakeScreen() {
  const { campaignId } = useParams<{ campaignId: string }>();
  const [orders, setOrders] = useState<Order[]>([]);
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);

  const loadOrders = () => {
    if (campaignId) {
      getOrders(campaignId).then((all) =>
        setOrders(all.filter((o) => o.orderType === "Unwashed" || o.orderType === "OutOfBounds"))
      );
    }
  };

  useEffect(loadOrders, [campaignId]);

  const selectedOrder = orders.find((o) => o.id === selectedOrderId) ?? null;

  const handleSelectOrder = (orderId: string) => {
    setSelectedOrderId((prev) => (prev === orderId ? null : orderId));
  };

  const handleStreetAdded = () => {
    setSelectedOrderId(null);
    loadOrders();
  };

  return (
    <div className="p-4">
      <h1 className="text-xl font-bold mb-4">Bestillinger til manuel behandling</h1>
      <div className={`flex gap-6 items-start ${selectedOrder?.orderType === "Unwashed" ? "flex-col md:flex-row" : ""}`}>
        <div className={selectedOrder?.orderType === "Unwashed" ? "w-full md:w-1/2" : "w-full"}>
          <OrderList
            orders={orders}
            selectedOrderId={selectedOrderId ?? undefined}
            onSelectOrder={handleSelectOrder}
          />
        </div>
        {selectedOrder?.orderType === "Unwashed" && (
          <div className="w-full md:w-1/2 border rounded p-4 bg-white">
            <h2 className="text-base font-semibold mb-4">Ret adresse</h2>
            <WashOrderForm
              order={selectedOrder}
              defaultZipCode={DEFAULT_ZIP_CODE}
              onStreetAdded={handleStreetAdded}
              onSubmit={() => {}}
            />
          </div>
        )}
      </div>
    </div>
  );
}
