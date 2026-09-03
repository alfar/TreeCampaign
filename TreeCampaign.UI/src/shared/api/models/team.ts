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
    Boogie: 'Boogietrailer',
    Large: 'Stor havetrailer',
};

export const trailerCapacity: Record<TrailerSize, number> = {
    Small: 8,
    Boogie: 16,
    Large: 12,
};

export const trailerSizeOrder: Record<TrailerSize, number> = {
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
    members: TeamMember[];
}
