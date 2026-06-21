export interface Address {
        displayName: string;
        latitude: number;
        longitude: number;
        streetSectionId: string;
}

export interface Stop {
    id: string;
    address: Address;
    amount: number;
    stopType: string;
    assignedTeamId?: string;
}
