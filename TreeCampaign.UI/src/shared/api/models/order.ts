export type OrderType = 'Incoming' | 'Unwashed' | 'Washed' | 'Validated' | 'OutOfBounds' | 'Transferred' | 'Settled';

export interface Order {
  id: string;
  orderType: OrderType;
  senderName: string;
  senderPhoneNumber?: string;
  amount: number;
  orderDate: string;
  message: string;
  address?: {
    displayName: string;
    houseNumber: string;
  };
  streetId?: string;
  houseNumber?: string;
  errorMessage?: string;
  territoryId?: string;
}
