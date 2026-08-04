export interface TeamMember {
    id: string;
    name: string;
    scoutRelativeName?: string;
    phoneNumber?: string;
}

export type TeamStatus = 'Active' | 'OnBreak';
export type TeamKind = 'Walking' | 'Trailer';
export type TrailerSize = 'Small' | 'Large' | 'Boogie';

export const trailerSizeLabels: Record<TrailerSize, string> = {
    Small: 'Lille havetrailer',
    Large: 'Stor havetrailer',
    Boogie: 'Boogietrailer',
};

export const trailerCapacity: Record<TrailerSize, number> = {
    Small: 8,
    Large: 12,
    Boogie: 16,
};

export const trailerSizeOrder: Record<TrailerSize, number> = {
    Small: 0,
    Large: 1,
    Boogie: 2,
};

export interface Team {
    id: string;
    name: string;
    status: TeamStatus;
    kind: TeamKind;
    isTrailerFull: boolean | null;
    trailerSize: TrailerSize | null;
    members: TeamMember[];
}
