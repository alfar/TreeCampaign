export interface TeamMember {
    id: string;
    name: string;
    scoutRelativeName?: string;
    phoneNumber: string;
}

export interface Team {
    id: string;
    name: string;
    members: TeamMember[];
}
