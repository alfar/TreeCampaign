import type { Order } from "../../shared/api/models/order";
import OrderCard from "./OrderCard";

interface OrderListProps {
  orders: Order[];
  selectedOrderId?: string;
  onSelectOrder?: (orderId: string) => void;
}

export default function OrderList({ orders, selectedOrderId, onSelectOrder }: OrderListProps) {
  if (orders.length === 0) {
    return <p className="text-sm text-gray-500">Ingen ordrer.</p>;
  }

  return (
    <div className="flex flex-col gap-3">
      {orders.map((order) => (
        <OrderCard
          key={order.id}
          order={order}
          selected={order.id === selectedOrderId}
          onClick={onSelectOrder ? () => onSelectOrder(order.id) : undefined}
        />
      ))}
    </div>
  );
}
