import type { Meta, StoryObj } from '@storybook/react-vite';

import NeighborhoodSection from '../features/dispatch/NeighborhoodSection';

const meta = {
  title: 'Dispatch/NeighborhoodSection',
  component: NeighborhoodSection,
  parameters: {
    // More on how to position stories at: https://storybook.js.org/docs/configure/story-layout
    layout: 'fullscreen',
  },
} satisfies Meta<typeof NeighborhoodSection>;

export default meta;
type Story = StoryObj<typeof meta>;

// More on component testing: https://storybook.js.org/docs/writing-tests/interaction-testing
export const Empty: Story = {
    args: {
        name: "Empty section"
    }
};
