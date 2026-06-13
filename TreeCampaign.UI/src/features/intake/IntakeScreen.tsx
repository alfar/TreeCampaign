import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getCampaigns, getOrders } from "../../shared/api/client";
import type { Order } from "../../shared/api/models/order";
import CreateStreetSectionForm from "./CreateStreetSectionForm";
import OrderList from "./OrderList";
import WashOrderForm from "./WashOrderForm";

const DEFAULT_ZIP_CODE = "8600";

export default function IntakeScreen() {
  const { campaignId } = useParams<{ campaignId: string }>();
  const [orders, setOrders] = useState<Order[]>([]);
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
  const [territoryId, setTerritoryId] = useState<string | null>(null);

  const loadOrders = () => {
    if (campaignId) {
      getOrders(campaignId).then((all) =>
        setOrders(all)
      );
    }
  };

  useEffect(loadOrders, [campaignId]);

  useEffect(() => {
    if (campaignId) {
      getCampaigns().then((all) => {
        const campaign = all.find((c) => c.id === campaignId);
        setTerritoryId(campaign?.territoryId ?? null);
      });
    }
  }, [campaignId]);

  const selectedOrder = orders.find((o) => o.id === selectedOrderId) ?? null;
  const showSidePanel = selectedOrder?.orderType === "Unwashed" || selectedOrder?.orderType === "OutOfBounds";

  const handleSelectOrder = (orderId: string) => {
    setSelectedOrderId((prev) => (prev === orderId ? null : orderId));
  };

  const handleStreetAdded = () => {
    setSelectedOrderId(null);
    loadOrders();
  };

  const handleWashed = () => {
    setOrders((prev) =>
      prev.map((o) => (o.id === selectedOrderId ? { ...o, orderType: "Washed" as const } : o))
    );
    setSelectedOrderId(null);
  };

  const handleSectionCreated = () => {
    setOrders((prev) => prev.filter((o) => o.id !== selectedOrderId));
    setSelectedOrderId(null);
  };

  return (
    <div className="p-4">
      <h1 className="text-xl font-bold mb-4">Bestillinger til manuel behandling</h1>
      <div className={`flex gap-6 items-start ${showSidePanel ? "flex-col md:flex-row" : ""}`}>
        <div className={showSidePanel ? "w-full md:w-1/2" : "w-full"}>
          <OrderList
            orders={orders.filter((o) => o.orderType === "Unwashed" || o.orderType === "OutOfBounds" || o.id === selectedOrderId)}
            selectedOrderId={selectedOrderId ?? undefined}
            onSelectOrder={handleSelectOrder}
          />
        </div>
        {selectedOrder?.orderType === "Unwashed" && (
          <div className="w-full md:w-1/2 border rounded p-4 bg-white">
            <h2 className="text-base font-semibold mb-4">Ret adresse</h2>
            <WashOrderForm
              order={selectedOrder}
              campaignId={campaignId!}
              defaultZipCode={DEFAULT_ZIP_CODE}
              onStreetAdded={handleStreetAdded}
              onWashed={handleWashed}
            />
          </div>
        )}
        {selectedOrder?.orderType === "OutOfBounds" && territoryId && selectedOrder.streetId && (
          <div className="w-full md:w-1/2 border rounded p-4 bg-white">
            <h2 className="text-base font-semibold mb-4">Opret vejstrækning</h2>
            <CreateStreetSectionForm
              order={{ ...selectedOrder, streetId: selectedOrder.streetId }}
              territoryId={territoryId}
              onSectionCreated={handleSectionCreated}
            />
          </div>
        )}
      </div>
    </div>
  );
}
