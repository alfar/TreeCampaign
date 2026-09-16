import type { Meta, StoryObj } from "@storybook/react-vite";

import StopTypeFilter from "./StopTypeFilter";
import type { StopType } from "../../shared/api/models/stop";

const meta = {
  title: "Dispatch/StopTypeFilter",
  component: StopTypeFilter,
} satisfies Meta<typeof StopTypeFilter>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Collapsed: Story = {
  args: {
    selectedStopTypes: new Set<StopType>(["Unassigned"]),
    setSelectedStopTypes: () => {},
  },
};

export const MultipleSelected: Story = {
  args: {
    selectedStopTypes: new Set<StopType>(["Unresolved", "Abandoned"]),
    setSelectedStopTypes: () => {},
  },
};

export const NoneSelected: Story = {
  args: {
    selectedStopTypes: new Set<StopType>(),
    setSelectedStopTypes: () => {},
  },
};
