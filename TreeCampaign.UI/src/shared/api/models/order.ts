export type OrderType = 'Incoming' | 'Unwashed' | 'Washed' | 'Validated' | 'OutOfBounds';

export interface Order {
  id: string;
  orderType: OrderType;
  senderName: string;
  senderPhoneNumber: string;
  amount: number;
  orderDate: string;
  message: string;
  address?: {
    displayName: string;
    houseNumber: string;
  };
  streetId?: string;
  houseNumber?: string;
}
