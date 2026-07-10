export interface TeamMember {
    id: string;
    name: string;
    scoutRelativeName?: string;
    phoneNumber: string;
}

export type TeamStatus = 'Active' | 'OnBreak';
export type TeamKind = 'Walking' | 'Trailer';

export interface Team {
    id: string;
    name: string;
    status: TeamStatus;
    kind: TeamKind;
    isTrailerFull: boolean | null;
    members: TeamMember[];
}
