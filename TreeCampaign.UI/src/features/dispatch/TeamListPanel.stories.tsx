import type { Meta, StoryObj } from "@storybook/react-vite";

import TeamListPanel from "./TeamListPanel";
import type { Stop } from "../../shared/api/models/stop";
import type { Team } from "../../shared/api/models/team";

const meta = {
  title: "Dispatch/TeamListPanel",
  component: TeamListPanel,
  parameters: {
    layout: "fullscreen",
  },
} satisfies Meta<typeof TeamListPanel>;

export default meta;
type Story = StoryObj<typeof meta>;

const teams: Team[] = [
  {
    id: "00000000-0000-0000-0000-000000000001",
    name: "Hold A",
    status: "Active",
    kind: "Trailer",
    isTrailerFull: false,
    trailerSize: "Small",
    currentExtraTrees: 0,
    totalExtraTrees: 0,
    members: [],
  },
  {
    id: "00000000-0000-0000-0000-000000000002",
    name: "Hold B",
    status: "OnBreak",
    kind: "Walking",
    isTrailerFull: null,
    trailerSize: null,
    currentExtraTrees: 2,
    totalExtraTrees: 2,
    members: [],
  },
];

const stops: Stop[] = [
  {
    id: "00000000-0000-0000-0000-000000000010",
    address: {
      streetSectionId: "00000000-0000-0000-0000-000000000020",
      displayName: "Vesterbakken 28",
      latitude: 55.6761,
      longitude: 12.5683,
    },
    amount: 2,
    stopType: "Assigned",
    assignedTeamId: teams[0].id,
  },
];

export const WithTeams: Story = {
  args: {
    campaignId: "00000000-0000-0000-0000-000000000000",
    sortedTeams: teams,
    stops,
    assignMode: false,
    teamExceedsSelection: () => false,
    onTeamClick: () => {},
    onUpdateStop: () => {},
    onUpdateTeam: () => {},
  },
};

export const AssignMode: Story = {
  args: {
    campaignId: "00000000-0000-0000-0000-000000000000",
    sortedTeams: teams,
    stops,
    assignMode: true,
    teamExceedsSelection: (team) => team.id === teams[1].id,
    onTeamClick: () => {},
    onUpdateStop: () => {},
    onUpdateTeam: () => {},
  },
};

export const NoTeams: Story = {
  args: {
    campaignId: "00000000-0000-0000-0000-000000000000",
    sortedTeams: [],
    stops: [],
    assignMode: false,
    teamExceedsSelection: () => false,
    onTeamClick: () => {},
    onUpdateStop: () => {},
    onUpdateTeam: () => {},
  },
};
