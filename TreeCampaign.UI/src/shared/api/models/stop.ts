export interface Address {
        displayName: string;
        latitude: number;
        longitude: number;
        streetSectionId: string;
}

export type StopType =
    | "Unassigned"
    | "Assigned"
    | "Collected"
    | "Unresolved"
    | "Delivered"
    | "Abandoned";

export const stopTypeLabels: Record<StopType, string> = {
    Unassigned: "Ikke tildelt",
    Assigned: "Tildelt",
    Collected: "Opsamlet",
    Unresolved: "Ikke fundet",
    Delivered: "Afleveret",
    Abandoned: "Opgivet",
};

export interface Stop {
    id: string;
    address: Address;
    amount: number;
    stopType: string;
    assignedTeamId?: string;
}
