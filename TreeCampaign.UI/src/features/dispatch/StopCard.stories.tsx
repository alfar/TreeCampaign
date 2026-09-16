import type { Meta, StoryObj } from '@storybook/react-vite';

import StopCard from './StopCard';

const meta = {
  title: 'Dispatch/StopCard',
  component: StopCard,
  parameters: {
    // More on how to position stories at: https://storybook.js.org/docs/configure/story-layout
    layout: 'fullscreen',
  },
} satisfies Meta<typeof StopCard>;

export default meta;
type Story = StoryObj<typeof meta>;

// More on component testing: https://storybook.js.org/docs/writing-tests/interaction-testing
export const Empty: Story = {
    args: {
        campaignId: "00000000-0000-0000-0000-000000000000",
        stop: {
            id: "00000000-0000-0000-0000-000000000001",
            address: { streetSectionId: "00000000-0000-0000-0000-000000000002", displayName: "Vesterbakken 28", latitude: 55.6761, longitude: 12.5683 },
            amount: 0,
            stopType: "Unassigned",
            assignedTeamId: undefined
        },
        teamName: undefined,
        assignMode: false,
        selected: false,
        onToggleSelect: undefined,
        onUpdateStop: undefined
    }
};

export const Assigned: Story = {
    args: {
        campaignId: "00000000-0000-0000-0000-000000000000",
        stop: {
            id: "00000000-0000-0000-0000-000000000001",
            address: { streetSectionId: "00000000-0000-0000-0000-000000000002", displayName: "Vesterbakken 28", latitude: 55.6761, longitude: 12.5683 },
            amount: 0,
            stopType: "Assigned",
            assignedTeamId: "00000000-0000-0000-0000-000000000003"
        },
        teamName: "Test team",
        assignMode: false,
        selected: false,
        onToggleSelect: undefined,
        onUpdateStop: undefined
    }
};
