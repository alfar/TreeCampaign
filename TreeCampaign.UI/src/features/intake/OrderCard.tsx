import type { Order, OrderType } from "../../shared/api/models/order";
import {
  CheckCircleIcon,
  ClockIcon,
  ExclamationCircleIcon,
  ExclamationTriangleIcon,
  WrenchScrewdriverIcon,
} from "@heroicons/react/24/solid";

interface OrderCardProps {
  order: Order;
  selected?: boolean;
  onClick?: () => void;
}

const STATE_CONFIG: Record<
  OrderType,
  { label: string; icon: React.ReactNode; borderColor: string }
> = {
  Incoming: {
    label: "Indkommende",
    icon: <ClockIcon className="w-5 h-5 text-gray-500" />,
    borderColor: "border-gray-300",
  },
  Unwashed: {
    label: "Ubehandlet",
    icon: <ExclamationTriangleIcon className="w-5 h-5 text-amber-500" />,
    borderColor: "border-amber-400",
  },
  Washed: {
    label: "Behandlet",
    icon: <WrenchScrewdriverIcon className="w-5 h-5 text-blue-500" />,
    borderColor: "border-blue-400",
  },
  Validated: {
    label: "Valideret",
    icon: <CheckCircleIcon className="w-5 h-5 text-green-600" />,
    borderColor: "border-green-500",
  },
  OutOfBounds: {
    label: "Uden for område",
    icon: <ExclamationCircleIcon className="w-5 h-5 text-red-500" />,
    borderColor: "border-red-400",
  },
};

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString("da-DK", {
    day: "numeric",
    month: "short",
    year: "numeric",
  });
}

export default function OrderCard({ order, selected, onClick }: OrderCardProps) {
  const state = STATE_CONFIG[order.orderType];
  const treeCount = Math.round(order.amount / 40);

  return (
    <div
      onClick={onClick}
      className={`p-4 border-l-4 rounded border ${state.borderColor} ${selected ? "bg-blue-50 ring-2 ring-blue-300" : "bg-white"} ${onClick ? "cursor-pointer" : ""}`}
    >
      <div className="flex items-start justify-between gap-2">
        <div className="flex-1 min-w-0">
          <div className="flex items-center gap-1.5 mb-1">
            {state.icon}
            <span className="text-xs text-gray-500">{state.label}</span>
          </div>
          <h2 className="text-base font-semibold truncate">{order.senderName}</h2>
          <p className="text-sm text-gray-600">{order.senderPhoneNumber}</p>
        </div>
        <div className="text-right shrink-0">
          <p className="text-base font-semibold">{order.amount} kr.</p>
          <p className="text-xs text-gray-500">{treeCount} {treeCount === 1 ? "træ" : "træer"}</p>
        </div>
      </div>

      {order.address && (
        <p className="mt-2 text-sm font-medium text-gray-800">
          {order.address.displayName} {order.address.houseNumber}
        </p>
      )}

      <p className="mt-2 text-sm text-gray-600 line-clamp-2">{order.message}</p>

      <p className="mt-2 text-xs text-gray-400">{formatDate(order.orderDate)}</p>
    </div>
  );
}
