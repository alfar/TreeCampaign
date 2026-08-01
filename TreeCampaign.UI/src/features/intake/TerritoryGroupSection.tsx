import {
  BuildingOffice2Icon,
  ChevronDownIcon,
  ChevronUpIcon,
} from "@heroicons/react/24/outline";
import { useState } from "react";
import type { Order } from "../../shared/api/models/order";
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
  const [expanded, setExpanded] = useState(true);

  return (
    <div className="rounded border border-gray-200">
      <div
        className={
          "bg-gray-100 p-2 flex justify-between items-center" +
          (expanded ? " rounded-t-sm" : " rounded")
        }
        onClick={() => setExpanded(!expanded)}
      >
        <div className="flex gap-2 items-center">
          <div className="rounded-full bg-blue-100 p-1">
            <BuildingOffice2Icon className="h-5 w-5 text-blue-600" />
          </div>
          <h2 className="text-lg text-gray-600">
            {name} ({orders.length})
            {unsettledAmount > 0 && (
              <span className="ml-2 text-sm font-normal text-gray-400">
                {unsettledAmount} kr. skal afregnes
              </span>
            )}
          </h2>
        </div>
        <div className="flex items-center gap-2">
          {hasUnsettled && (
            <button
              type="button"
              onClick={(e) => {
                e.stopPropagation();
                onSettleAll();
              }}
              disabled={isSettling}
              className="text-xs bg-green-600 text-white py-1 px-3 rounded disabled:opacity-40"
            >
              {isSettling ? "Markerer…" : "Marker alle som afregnet"}
            </button>
          )}
          {expanded ? (
            <ChevronDownIcon className="h-5 w-5" />
          ) : (
            <ChevronUpIcon className="h-5 w-5" />
          )}
        </div>
      </div>
      <div className={expanded ? "rounded-b-sm p-2" : "overflow-hidden max-h-0"}>
        {error && <p className="text-sm text-red-600 mb-2">{error}</p>}
        <OrderList
          orders={orders}
          selectedOrderId={selectedOrderId}
          onSelectOrder={onSelectOrder}
          territoryNameById={territoryNameById}
        />
      </div>
    </div>
  );
}
