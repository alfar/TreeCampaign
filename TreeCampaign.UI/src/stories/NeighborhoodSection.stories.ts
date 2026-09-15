import type { Meta, StoryObj } from "@storybook/react-vite";

import NeighborhoodSection from "../features/dispatch/NeighborhoodSection";

const meta = {
  title: "Dispatch/NeighborhoodSection",
  component: NeighborhoodSection,
  parameters: {
    // More on how to position stories at: https://storybook.js.org/docs/configure/story-layout
    layout: "fullscreen",
  },
} satisfies Meta<typeof NeighborhoodSection>;

export default meta;
type Story = StoryObj<typeof meta>;

const getTeamName = (id: string | undefined) => (id ? `Team ${id}` : undefined);

// More on component testing: https://storybook.js.org/docs/writing-tests/interaction-testing
export const Empty: Story = {
  args: {
    name: "Empty section",
    campaignId: "00000000-0000-0000-0000-000000000000",
    selectedStopIds: new Set<string>(),
    stops: [],
    toggleStop: (_: string) => {},
    getTeamName,
  },
};

// More on component testing: https://storybook.js.org/docs/writing-tests/interaction-testing
export const TwoStops: Story = {
  args: {
    name: "Two stops section",
    campaignId: "00000000-0000-0000-0000-000000000000",
    selectedStopIds: new Set<string>(),
    stops: [
      {
        id: "00000000-0000-0000-0000-000000000001",
        address: {
          streetSectionId: "00000000-0000-0000-0000-000000000002",
          displayName: "Vesterbakken 28",
          latitude: 55.6761,
          longitude: 12.5683,
        },
        amount: 0,
        stopType: "Unassigned",
        assignedTeamId: undefined,
      },
      {
        id: "00000000-0000-0000-0000-000000000003",
        address: {
          streetSectionId: "00000000-0000-0000-0000-000000000004",
          displayName: "Vesterbakken 30",
          latitude: 55.6761,
          longitude: 12.5683,
        },
        amount: 0,
        stopType: "Assigned",
        assignedTeamId: "00000000-0000-0000-0000-000000000005",
      },
    ],
    toggleStop: (_: string) => {},
    getTeamName,
  },
};
