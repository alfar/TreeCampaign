export interface TeamMember {
    id: string;
    name: string;
    scoutRelativeName?: string;
    phoneNumber: string;
}

export type TeamStatus = 'Active' | 'OnBreak' | 'TrailerFull';

export interface Team {
    id: string;
    name: string;
    status: TeamStatus;
    members: TeamMember[];
}
