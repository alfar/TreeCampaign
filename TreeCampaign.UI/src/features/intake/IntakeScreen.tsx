import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getCampaigns, getOrders, getTerritories } from "../../shared/api/client";
import type { Order } from "../../shared/api/models/order";
import type { Territory } from "../../shared/api/models/territory";
import CreateOrderForm from "./CreateOrderForm";
import CreateStreetSectionForm from "./CreateStreetSectionForm";
import ImportPaymentsForm from "./ImportPaymentsForm";
import OrderList from "./OrderList";
import TransferOrderForm from "./TransferOrderForm";
import UndoTransferForm from "./UndoTransferForm";
import WashOrderForm from "./WashOrderForm";
import NavigationPage from "../../shared/components/NavigationPage";

const DEFAULT_ZIP_CODE = "8600";

export default function IntakeScreen() {
  const { campaignId } = useParams<{ campaignId: string }>();
  const [orders, setOrders] = useState<Order[]>([]);
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
  const [territoryId, setTerritoryId] = useState<string | null>(null);
  const [territories, setTerritories] = useState<Territory[]>([]);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [showImportForm, setShowImportForm] = useState(false);
  const [activeTab, setActiveTab] = useState<"pending" | "transferred">("pending");
  const [oobAction, setOobAction] = useState<"section" | "transfer">("section");

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

  useEffect(() => {
    getTerritories().then(setTerritories);
  }, []);

  const territoryNameById = territories.reduce<Record<string, string>>((acc, t) => {
    acc[t.id] = t.name;
    return acc;
  }, {});

  useEffect(() => {
    if (!campaignId) return;

    const es = new EventSource(`/api/campaigns/${campaignId}/events`);

    es.addEventListener("intake-update", (e: MessageEvent) => {
      const { type, data } = JSON.parse(e.data) as {
        type: string;
        data: Record<string, unknown>;
      };

      const patchOrderFunc = (orderId: string, patch: Partial<Order>) => {
        return () => {
          setOrders((prev) =>
            prev.map((o) => (o.id === orderId ? { ...o, ...patch } : o)),
          );
        };
      };

      const actionByEvent: Record<string, () => void> = {
        OrderReceived: () => {
          const sender = data.sender as { name: string; phoneNumber: string };
          setOrders((prev) => [
            ...prev.filter((o) => o.id !== (data.id as string)),
            {
              id: data.id as string,
              orderType: "Incoming",
              senderName: sender.name,
              senderPhoneNumber: sender.phoneNumber,
              amount: data.amount as number,
              orderDate: data.orderDate as string,
              message: data.message as string,
            },
          ]);
        },
        OrderMarkedUnwashed: patchOrderFunc(data.id as string, {
          orderType: "Unwashed",
          errorMessage: data.errorMessage as string | undefined,
        }),
        OrderValidated: patchOrderFunc(data.id as string, {
          orderType: "Validated",
        }),
        OrderWashed: patchOrderFunc(data.id as string, { orderType: "Washed" }),
        OrderMarkedOutOfBounds: patchOrderFunc(data.id as string, {
          orderType: "OutOfBounds",
          streetId: data.streetId as string,
          houseNumber: data.houseNumber as string,
        }),
        OrderTransferred: patchOrderFunc(data.id as string, {
          orderType: "Transferred",
          territoryId: data.territoryId as string,
        }),
        OrderTransferUndone: patchOrderFunc(data.id as string, {
          orderType: "OutOfBounds",
          territoryId: undefined,
        }),
        OrderSettled: patchOrderFunc(data.id as string, {
          orderType: "Settled",
        }),
      };

      const action = actionByEvent[type];
      if (action !== undefined) {
        action();
      }
    });

    return () => es.close();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [campaignId]);

  const selectedOrder = orders.find((o) => o.id === selectedOrderId) ?? null;
  const showSidePanel =
    showCreateForm ||
    showImportForm ||
    selectedOrder?.orderType === "Unwashed" ||
    selectedOrder?.orderType === "OutOfBounds" ||
    selectedOrder?.orderType === "Transferred";

  const handleSelectOrder = (orderId: string) => {
    setShowCreateForm(false);
    setShowImportForm(false);
    setOobAction("section");
    setSelectedOrderId((prev) => (prev === orderId ? null : orderId));
  };

  const handleOpenCreateForm = () => {
    setSelectedOrderId(null);
    setShowImportForm(false);
    setShowCreateForm(true);
  };

  const handleOpenImportForm = () => {
    setSelectedOrderId(null);
    setShowCreateForm(false);
    setShowImportForm(true);
  };

  const handleWashed = () => {
    setSelectedOrderId(null);
  };

  const handleSectionCreated = () => {
    setSelectedOrderId(null);
  };

  const handleTransferred = () => {
    setSelectedOrderId(null);
  };

  const handleUndone = () => {
    setSelectedOrderId(null);
  };

  const handleSettled = () => {
    setSelectedOrderId(null);
  };

  const handleOrderCreated = () => {
    setShowCreateForm(false);
  };

  const handleSelectTab = (tab: "pending" | "transferred") => {
    setSelectedOrderId(null);
    setShowCreateForm(false);
    setShowImportForm(false);
    setActiveTab(tab);
  };

  return (
    <NavigationPage>
      <div className="p-4">
        <div className="flex items-center justify-between mb-4">
          <h1 className="text-xl font-bold">
            Bestillinger til manuel behandling
          </h1>
          <div className="flex gap-2">
            <button
              onClick={handleOpenImportForm}
              className="bg-gray-100 text-gray-700 text-sm py-1.5 px-4 rounded hover:bg-gray-200 border"
            >
              Importér CSV
            </button>
            <button
              onClick={handleOpenCreateForm}
              className="bg-blue-600 text-white text-sm py-1.5 px-4 rounded hover:bg-blue-700"
            >
              Ny bestilling
            </button>
          </div>
        </div>
        <div className="flex gap-2 mb-4 border-b">
          <button
            onClick={() => handleSelectTab("pending")}
            className={`text-sm py-2 px-4 border-b-2 -mb-px ${activeTab === "pending" ? "border-blue-600 text-blue-600 font-medium" : "border-transparent text-gray-500 hover:text-gray-700"}`}
          >
            Til behandling
          </button>
          <button
            onClick={() => handleSelectTab("transferred")}
            className={`text-sm py-2 px-4 border-b-2 -mb-px ${activeTab === "transferred" ? "border-blue-600 text-blue-600 font-medium" : "border-transparent text-gray-500 hover:text-gray-700"}`}
          >
            Overførte
          </button>
        </div>
        <div
          className={`flex gap-6 items-start ${showSidePanel ? "flex-col md:flex-row" : ""}`}
        >
          <div className={showSidePanel ? "w-full md:w-1/2" : "w-full"}>
            <OrderList
              orders={
                activeTab === "pending"
                  ? orders.filter(
                      (o) =>
                        o.orderType === "Unwashed" ||
                        o.orderType === "OutOfBounds" ||
                        o.id === selectedOrderId,
                    )
                  : orders.filter(
                      (o) => o.orderType === "Transferred" || o.orderType === "Settled",
                    )
              }
              selectedOrderId={selectedOrderId ?? undefined}
              onSelectOrder={handleSelectOrder}
              territoryNameById={territoryNameById}
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
          {showImportForm && (
            <div className="w-full md:w-1/2 border rounded p-4 bg-white">
              <h2 className="text-base font-semibold mb-4">Importér betalinger</h2>
              <ImportPaymentsForm campaignId={campaignId!} onImported={loadOrders} />
            </div>
          )}
          {!showCreateForm && !showImportForm && selectedOrder?.orderType === "Unwashed" && (
            <div className="w-full md:w-1/2 border rounded p-4 bg-white">
              <h2 className="text-base font-semibold mb-4">Ret adresse</h2>
              <WashOrderForm
                order={selectedOrder}
                campaignId={campaignId!}
                defaultZipCode={DEFAULT_ZIP_CODE}
                onWashed={handleWashed}
              />
            </div>
          )}
          {!showCreateForm &&
            !showImportForm &&
            selectedOrder?.orderType === "OutOfBounds" &&
            territoryId &&
            selectedOrder.streetId && (
              <div className="w-full md:w-1/2 border rounded p-4 bg-white">
                <div className="flex gap-2 mb-4">
                  <button
                    type="button"
                    onClick={() => setOobAction("section")}
                    className={`text-sm py-1.5 px-3 rounded border ${oobAction === "section" ? "bg-blue-600 text-white border-blue-600" : "bg-white text-gray-700 hover:bg-gray-50"}`}
                  >
                    Opret vejstrækning
                  </button>
                  <button
                    type="button"
                    onClick={() => setOobAction("transfer")}
                    className={`text-sm py-1.5 px-3 rounded border ${oobAction === "transfer" ? "bg-blue-600 text-white border-blue-600" : "bg-white text-gray-700 hover:bg-gray-50"}`}
                  >
                    Overfør til andet område
                  </button>
                </div>
                {oobAction === "section" ? (
                  <CreateStreetSectionForm
                    order={{ ...selectedOrder, streetId: selectedOrder.streetId }}
                    territoryId={territoryId}
                    onSectionCreated={handleSectionCreated}
                  />
                ) : (
                  <TransferOrderForm
                    order={selectedOrder}
                    campaignId={campaignId!}
                    currentTerritoryId={territoryId}
                    onTransferred={handleTransferred}
                  />
                )}
              </div>
            )}
          {!showCreateForm &&
            !showImportForm &&
            selectedOrder?.orderType === "Transferred" && (
              <div className="w-full md:w-1/2 border rounded p-4 bg-white">
                <h2 className="text-base font-semibold mb-4">Overført bestilling</h2>
                <UndoTransferForm
                  order={selectedOrder}
                  campaignId={campaignId!}
                  territoryName={
                    selectedOrder.territoryId
                      ? territoryNameById[selectedOrder.territoryId]
                      : undefined
                  }
                  onUndone={handleUndone}
                  onSettled={handleSettled}
                />
              </div>
            )}
        </div>
      </div>
    </NavigationPage>
  );
}
