import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getCampaigns, getOrders } from "../../shared/api/client";
import type { Order } from "../../shared/api/models/order";
import CreateOrderForm from "./CreateOrderForm";
import CreateStreetSectionForm from "./CreateStreetSectionForm";
import OrderList from "./OrderList";
import WashOrderForm from "./WashOrderForm";
import NavigationPage from "../../shared/components/NavigationPage";

const DEFAULT_ZIP_CODE = "8600";

export default function IntakeScreen() {
  const { campaignId } = useParams<{ campaignId: string }>();
  const [orders, setOrders] = useState<Order[]>([]);
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
  const [territoryId, setTerritoryId] = useState<string | null>(null);
  const [showCreateForm, setShowCreateForm] = useState(false);

  const loadOrders = () => {
    if (campaignId) {
      getOrders(campaignId).then(setOrders);
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
  const showSidePanel =
    showCreateForm ||
    selectedOrder?.orderType === "Unwashed" ||
    selectedOrder?.orderType === "OutOfBounds";

  const handleSelectOrder = (orderId: string) => {
    setShowCreateForm(false);
    setSelectedOrderId((prev) => (prev === orderId ? null : orderId));
  };

  const handleOpenCreateForm = () => {
    setSelectedOrderId(null);
    setShowCreateForm(true);
  };

  const handleStreetAdded = () => {
    loadOrders();
  };

  const handleWashed = () => {
    setOrders((prev) =>
      prev.map((o) =>
        o.id === selectedOrderId ? { ...o, orderType: "Washed" as const } : o,
      ),
    );
    setSelectedOrderId(null);
  };

  const handleSectionCreated = () => {
    setOrders((prev) => prev.filter((o) => o.id !== selectedOrderId));
    setSelectedOrderId(null);
  };

  const handleOrderCreated = () => {
    setShowCreateForm(false);
    loadOrders();
  };

  return (
    <NavigationPage>
      <div className="p-4">
        <div className="flex items-center justify-between mb-4">
          <h1 className="text-xl font-bold">
            Bestillinger til manuel behandling
          </h1>
          <button
            onClick={handleOpenCreateForm}
            className="bg-blue-600 text-white text-sm py-1.5 px-4 rounded hover:bg-blue-700"
          >
            Ny bestilling
          </button>
        </div>
        <div
          className={`flex gap-6 items-start ${showSidePanel ? "flex-col md:flex-row" : ""}`}
        >
          <div className={showSidePanel ? "w-full md:w-1/2" : "w-full"}>
            <OrderList
              orders={orders.filter(
                (o) =>
                  o.orderType === "Unwashed" ||
                  o.orderType === "OutOfBounds" ||
                  o.id === selectedOrderId,
              )}
              selectedOrderId={selectedOrderId ?? undefined}
              onSelectOrder={handleSelectOrder}
            />
          </div>
          {showCreateForm && (
            <div className="w-full md:w-1/2 border rounded p-4 bg-white">
              <h2 className="text-base font-semibold mb-4">Ny bestilling</h2>
              <CreateOrderForm
                campaignId={campaignId!}
                defaultZipCode={DEFAULT_ZIP_CODE}
                onOrderCreated={handleOrderCreated}
              />
            </div>
          )}
          {!showCreateForm && selectedOrder?.orderType === "Unwashed" && (
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
          {!showCreateForm &&
            selectedOrder?.orderType === "OutOfBounds" &&
            territoryId &&
            selectedOrder.streetId && (
              <div className="w-full md:w-1/2 border rounded p-4 bg-white">
                <h2 className="text-base font-semibold mb-4">
                  Opret vejstrækning
                </h2>
                <CreateStreetSectionForm
                  order={{ ...selectedOrder, streetId: selectedOrder.streetId }}
                  territoryId={territoryId}
                  onSectionCreated={handleSectionCreated}
                />
              </div>
            )}
        </div>
      </div>
    </NavigationPage>
  );
}
