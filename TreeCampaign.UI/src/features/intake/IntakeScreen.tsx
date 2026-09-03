import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getCampaigns, getOrders, getTerritories, markOrderUnwashable, revalidateCampaignOrders, settleTerritoryOrders } from "../../shared/api/client";
import type { Order } from "../../shared/api/models/order";
import type { Territory } from "../../shared/api/models/territory";
import CreateOrderForm from "./CreateOrderForm";
import CreateStreetSectionForm from "./CreateStreetSectionForm";
import ImportPaymentsForm from "./ImportPaymentsForm";
import OrderList from "./OrderList";
import TerritoryGroupSection from "./TerritoryGroupSection";
import TransferOrderForm from "./TransferOrderForm";
import UndoTransferForm from "./UndoTransferForm";
import UnwashableOrderForm from "./UnwashableOrderForm";
import WashOrderForm from "./WashOrderForm";
import NavigationPage from "../../shared/components/NavigationPage";
import Button from "../../components/Button";

const DEFAULT_ZIP_CODE = "8600";

export default function IntakeScreen() {
  const { campaignId } = useParams<{ campaignId: string }>();
  const [orders, setOrders] = useState<Order[]>([]);
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
  const [territoryId, setTerritoryId] = useState<string | null>(null);
  const [territories, setTerritories] = useState<Territory[]>([]);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [showImportForm, setShowImportForm] = useState(false);
  const [activeTab, setActiveTab] = useState<"pending" | "transferred" | "unusable">("pending");
  const [oobAction, setOobAction] = useState<"section" | "transfer" | "unwashable">("section");
  const [bulkSettling, setBulkSettling] = useState<Record<string, boolean>>({});
  const [bulkError, setBulkError] = useState<Record<string, string | undefined>>({});
  const [revalidating, setRevalidating] = useState(false);
  const [markingUnwashable, setMarkingUnwashable] = useState(false);

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
        OrderMarkedUnwashable: patchOrderFunc(data.id as string, {
          orderType: "Unwashable",
        }),
        OrderUnwashableUndone: patchOrderFunc(data.id as string, {
          orderType: "Unwashed",
        }),
        OrderRefunded: patchOrderFunc(data.id as string, {
          orderType: "Refunded",
        }),
        OrderMarkedAsDonation: patchOrderFunc(data.id as string, {
          orderType: "Donated",
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

  const transferredGroups = Object.values(
    orders
      .filter((o) => o.orderType === "Transferred" || o.orderType === "Settled")
      .reduce<Record<string, Order[]>>((acc, o) => {
        const key = o.territoryId ?? "unknown";
        (acc[key] ??= []).push(o);
        return acc;
      }, {}),
  )
    .map((group) => ({
      territoryId: group[0].territoryId,
      orders: [...group].sort((a, b) =>
        a.orderType === b.orderType ? 0 : a.orderType === "Transferred" ? -1 : 1,
      ),
      unsettledAmount: group
        .filter((o) => o.orderType === "Transferred")
        .reduce((sum, o) => sum + o.amount, 0),
    }))
    .sort((a, b) =>
      (a.territoryId ? territoryNameById[a.territoryId] : undefined)?.localeCompare(
        (b.territoryId ? territoryNameById[b.territoryId] : undefined) ?? "",
      ) ?? 0,
    );

  const selectedOrder = orders.find((o) => o.id === selectedOrderId) ?? null;
  const showSidePanel =
    showCreateForm ||
    showImportForm ||
    selectedOrder?.orderType === "Unwashed" ||
    selectedOrder?.orderType === "OutOfBounds" ||
    selectedOrder?.orderType === "Transferred" ||
    selectedOrder?.orderType === "Unwashable";

  const unusableOrderRank = (orderType: Order["orderType"]) =>
    orderType === "Unwashable" ? 0 : 1;

  const unusableOrders = orders
    .filter(
      (o) => o.orderType === "Unwashable" || o.orderType === "Refunded" || o.orderType === "Donated",
    )
    .sort((a, b) => {
      const rankDiff = unusableOrderRank(a.orderType) - unusableOrderRank(b.orderType);
      return rankDiff !== 0
        ? rankDiff
        : new Date(a.orderDate).getTime() - new Date(b.orderDate).getTime();
    });
  const outstandingUnusableCount = unusableOrders.filter((o) => o.orderType === "Unwashable").length;
  const donatedAmount = unusableOrders
    .filter((o) => o.orderType === "Donated")
    .reduce((sum, o) => sum + o.amount, 0);

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

  const handleMarkedUnwashable = () => {
    setSelectedOrderId(null);
  };

  const handleUnwashableResolved = () => {
    setSelectedOrderId(null);
  };

  const handleMarkOutOfBoundsUnwashable = async () => {
    if (!campaignId || !selectedOrder) return;
    setMarkingUnwashable(true);
    try {
      const res = await markOrderUnwashable(campaignId, selectedOrder.id);
      if (res.ok) {
        setSelectedOrderId(null);
      }
    } finally {
      setMarkingUnwashable(false);
    }
  };

  const handleSettleTerritory = async (settleTerritoryId: string) => {
    setBulkSettling((prev) => ({ ...prev, [settleTerritoryId]: true }));
    setBulkError((prev) => ({ ...prev, [settleTerritoryId]: undefined }));
    try {
      const res = await settleTerritoryOrders(campaignId!, settleTerritoryId);
      if (res.ok) {
        const settled: { id: string }[] = await res.json();
        const settledIds = new Set(settled.map((o) => o.id));
        setOrders((prev) =>
          prev.map((o) => (settledIds.has(o.id) ? { ...o, orderType: "Settled" } : o)),
        );
      } else {
        setBulkError((prev) => ({ ...prev, [settleTerritoryId]: "Noget gik galt. Prøv igen." }));
      }
    } finally {
      setBulkSettling((prev) => ({ ...prev, [settleTerritoryId]: false }));
    }
  };

  const handleOrderCreated = () => {
    setShowCreateForm(false);
  };

  const handleRevalidate = async () => {
    if (!campaignId) return;
    setRevalidating(true);
    try {
      await revalidateCampaignOrders(campaignId);
    } finally {
      setRevalidating(false);
    }
  };

  const handleSelectTab = (tab: "pending" | "transferred" | "unusable") => {
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
            <Button variant="secondary" onClick={handleRevalidate} disabled={revalidating}>
              {revalidating ? "Genbehandler…" : "Genbehandl bestillinger"}
            </Button>
            <Button variant="secondary" onClick={handleOpenImportForm}>
              Importér CSV
            </Button>
            <Button onClick={handleOpenCreateForm}>Ny bestilling</Button>
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
            {transferredGroups.length > 0 && (
              <span className="ml-1 text-gray-500">({transferredGroups.length})</span>
            )}
          </button>
          <button
            onClick={() => handleSelectTab("unusable")}
            className={`text-sm py-2 px-4 border-b-2 -mb-px ${activeTab === "unusable" ? "border-blue-600 text-blue-600 font-medium" : "border-transparent text-gray-500 hover:text-gray-700"}`}
          >
            Ikke brugbare
            {outstandingUnusableCount > 0 && (
              <span className="ml-1 text-gray-500">({outstandingUnusableCount})</span>
            )}
          </button>
        </div>
        <div
          className={`flex gap-6 items-start ${showSidePanel ? "flex-col md:flex-row" : ""}`}
        >
          <div className={showSidePanel ? "w-full md:w-1/2" : "w-full"}>
            {activeTab === "pending" ? (
              <OrderList
                orders={orders
                  .filter(
                    (o) =>
                      o.orderType === "Unwashed" ||
                      o.orderType === "OutOfBounds" ||
                      o.id === selectedOrderId,
                  )}
                selectedOrderId={selectedOrderId ?? undefined}
                onSelectOrder={handleSelectOrder}
                territoryNameById={territoryNameById}
              />
            ) : activeTab === "transferred" ? (
              transferredGroups.length === 0 ? (
                <p className="text-sm text-gray-500">Ingen ordrer.</p>
              ) : (
                <div className="flex flex-col gap-4">
                  {transferredGroups.map((group) => (
                    <TerritoryGroupSection
                      key={group.territoryId ?? "unknown"}
                      name={group.territoryId ? (territoryNameById[group.territoryId] ?? "Ukendt område") : "Ukendt område"}
                      orders={group.orders}
                      unsettledAmount={group.unsettledAmount}
                      hasUnsettled={group.orders.some((o) => o.orderType === "Transferred")}
                      isSettling={group.territoryId ? (bulkSettling[group.territoryId] ?? false) : false}
                      error={group.territoryId ? bulkError[group.territoryId] : undefined}
                      onSettleAll={() => group.territoryId && handleSettleTerritory(group.territoryId)}
                      selectedOrderId={selectedOrderId ?? undefined}
                      onSelectOrder={handleSelectOrder}
                      territoryNameById={territoryNameById}
                    />
                  ))}
                </div>
              )
            ) : (
              <div className="flex flex-col gap-2">
                {donatedAmount > 0 && (
                  <p className="text-sm text-gray-600">
                    Doneret i alt: <span className="font-medium">{donatedAmount} kr.</span>
                  </p>
                )}
                <OrderList
                  orders={unusableOrders}
                  selectedOrderId={selectedOrderId ?? undefined}
                  onSelectOrder={handleSelectOrder}
                  territoryNameById={territoryNameById}
                />
              </div>
            )}
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
                onMarkedUnwashable={handleMarkedUnwashable}
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
                  <Button
                    variant={oobAction === "section" ? "primary" : "secondary"}
                    onClick={() => setOobAction("section")}
                  >
                    Opret vejstrækning
                  </Button>
                  <Button
                    variant={oobAction === "transfer" ? "primary" : "secondary"}
                    onClick={() => setOobAction("transfer")}
                  >
                    Overfør til andet område
                  </Button>
                  <Button
                    variant={oobAction === "unwashable" ? "primary" : "secondary"}
                    onClick={() => setOobAction("unwashable")}
                  >
                    Ikke brugbar
                  </Button>
                </div>
                {oobAction === "section" ? (
                  <CreateStreetSectionForm
                    order={{ ...selectedOrder, streetId: selectedOrder.streetId }}
                    territoryId={territoryId}
                    onSectionCreated={handleSectionCreated}
                  />
                ) : oobAction === "transfer" ? (
                  <TransferOrderForm
                    order={selectedOrder}
                    campaignId={campaignId!}
                    currentTerritoryId={territoryId}
                    onTransferred={handleTransferred}
                  />
                ) : (
                  <div className="flex flex-col gap-4">
                    <p className="text-sm text-gray-600">
                      Denne bestilling bliver ikke til et stop og flyttes til fanen "Ikke brugbare".
                    </p>
                    <Button
                      variant="danger"
                      onClick={handleMarkOutOfBoundsUnwashable}
                      disabled={markingUnwashable}
                      className="self-start"
                    >
                      {markingUnwashable ? "Markerer…" : "Marker som ikke brugbar"}
                    </Button>
                  </div>
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
          {!showCreateForm &&
            !showImportForm &&
            selectedOrder?.orderType === "Unwashable" && (
              <div className="w-full md:w-1/2 border rounded p-4 bg-white">
                <h2 className="text-base font-semibold mb-4">Ikke brugbar bestilling</h2>
                <UnwashableOrderForm
                  order={selectedOrder}
                  campaignId={campaignId!}
                  onRefunded={handleUnwashableResolved}
                  onDonated={handleUnwashableResolved}
                  onUndone={handleUnwashableResolved}
                />
              </div>
            )}
        </div>
      </div>
    </NavigationPage>
  );
}
