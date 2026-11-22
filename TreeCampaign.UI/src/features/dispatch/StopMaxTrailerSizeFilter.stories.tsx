import type { Meta, StoryObj } from "@storybook/react-vite";

import StopMaxTrailerSizeFilter, {
  allMaxTrailerSizes,
} from "./StopMaxTrailerSizeFilter";
import type { TrailerSize } from "../../shared/api/models/team";

const meta = {
  title: "Dispatch/StopMaxTrailerSizeFilter",
  component: StopMaxTrailerSizeFilter,
} satisfies Meta<typeof StopMaxTrailerSizeFilter>;

export default meta;
type Story = StoryObj<typeof meta>;

export const AllSelected: Story = {
  args: {
    selectedSizes: new Set<TrailerSize>(allMaxTrailerSizes),
    setSelectedSizes: () => {},
  },
};

export const LargeOnly: Story = {
  args: {
    selectedSizes: new Set<TrailerSize>(["Large"]),
    setSelectedSizes: () => {},
  },
};
