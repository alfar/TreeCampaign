export interface TeamMember {
    id: string;
    name: string;
    scoutRelativeName?: string;
    phoneNumber?: string;
}

export type TeamStatus = 'Active' | 'OnBreak';
export type TeamKind = 'Walking' | 'Trailer';
/** 'Runner' is only valid as a street section's max size (no trailer, gathered on foot), never as a team's trailer. */
export type TrailerSize = 'Runner' | 'Small' | 'Boogie' | 'Large';

export const trailerSizeLabels: Record<TrailerSize, string> = {
    Runner: 'Løber (ingen trailer)',
    Small: 'Lille havetrailer',
    Boogie: 'Boogietrailer',
    Large: 'Stor trailer',
};

/** Sizes a team's trailer can actually have (excludes 'Runner'). */
export const teamTrailerSizes: TrailerSize[] = ['Small', 'Boogie', 'Large'];

export const trailerCapacity: Record<TrailerSize, number> = {
    Runner: 0,
    Small: 8,
    Boogie: 16,
    Large: 12,
};

export const trailerSizeOrder: Record<TrailerSize, number> = {
    Runner: -1,
    Small: 0,
    Boogie: 1,
    Large: 2,
};

export interface Team {
    id: string;
    name: string;
    status: TeamStatus;
    kind: TeamKind;
    isTrailerFull: boolean | null;
    trailerSize: TrailerSize | null;
    currentExtraTrees: number;
    totalExtraTrees: number;
    members: TeamMember[];
}
