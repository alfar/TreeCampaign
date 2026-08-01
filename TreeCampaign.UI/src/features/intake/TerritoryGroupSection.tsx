import { BuildingOffice2Icon } from "@heroicons/react/24/outline";
import type { Order } from "../../shared/api/models/order";
import Section from "../../shared/components/Section";
import OrderList from "./OrderList";

interface TerritoryGroupSectionProps {
  name: string;
  orders: Order[];
  unsettledAmount: number;
  hasUnsettled: boolean;
  isSettling: boolean;
  error?: string;
  onSettleAll: () => void;
  selectedOrderId?: string;
  onSelectOrder: (orderId: string) => void;
  territoryNameById: Record<string, string>;
}

export default function TerritoryGroupSection({
  name,
  orders,
  unsettledAmount,
  hasUnsettled,
  isSettling,
  error,
  onSettleAll,
  selectedOrderId,
  onSelectOrder,
  territoryNameById,
}: TerritoryGroupSectionProps) {
  return (
    <Section
      icon={<BuildingOffice2Icon className="h-5 w-5 text-blue-600" />}
      title={
        <>
          {name} ({orders.length})
          {unsettledAmount > 0 && (
            <span className="ml-2 text-sm font-normal text-gray-400">
              {unsettledAmount} kr. skal afregnes
            </span>
          )}
        </>
      }
      actions={
        hasUnsettled && (
          <button
            type="button"
            onClick={onSettleAll}
            disabled={isSettling}
            className="text-xs bg-green-600 text-white py-1 px-3 rounded disabled:opacity-40"
          >
            {isSettling ? "Markerer…" : "Marker alle som afregnet"}
          </button>
        )
      }
    >
      {error && <p className="text-sm text-red-600 mb-2">{error}</p>}
      <OrderList
        orders={orders}
        selectedOrderId={selectedOrderId}
        onSelectOrder={onSelectOrder}
        territoryNameById={territoryNameById}
      />
    </Section>
  );
}
