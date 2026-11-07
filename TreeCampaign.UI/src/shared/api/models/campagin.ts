export interface Campaign {
    id: string;
    season: number;
    territoryId?: string;
    collectionDate?: string | null;
    pricePerTree?: number | null;
    title?: string | null;
    lastOrderDateTime?: string | null;
}

export interface CampaignInput {
    year: number;
    territoryId?: string;
    collectionDate?: string | null;
    pricePerTree?: number | null;
    title?: string | null;
    lastOrderDateTime?: string | null;
}
