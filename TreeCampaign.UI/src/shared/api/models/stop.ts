export interface Address {
        displayName: string;
        latitude: number;
        longitude: number;
}

export interface Stop {
    id: string;
    address: Address;
    amount: number;
    stopType: string;
    assignedTeamId?: string;
}
