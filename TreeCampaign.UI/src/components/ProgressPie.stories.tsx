import type { Meta, StoryObj } from "@storybook/react-vite";

import ProgressPie from "./ProgressPie";

const meta = {
  title: "Components/ProgressPie",
  component: ProgressPie,
  decorators: [
    (Story) => (
      <div style={{ width: 240 }}>
        <Story />
      </div>
    ),
  ],
} satisfies Meta<typeof ProgressPie>;

export default meta;
type Story = StoryObj<typeof meta>;

export const FourSections: Story = {
  args: {
    parts: [
      { title: "Collected", amount: 120, color: "#16a34a" },
      { title: "Assigned", amount: 60, color: "#2563eb" },
      { title: "Unassigned", amount: 45, color: "#f59e0b" },
      { title: "Unresolved", amount: 15, color: "#dc2626" },
    ],
  },
};

export const FiveSectionsEvenSplit: Story = {
  args: {
    parts: [
      { title: "Collected", amount: 20, color: "#16a34a" },
      { title: "Assigned", amount: 20, color: "#2563eb" },
      { title: "Unassigned", amount: 20, color: "#f59e0b" },
      { title: "Unresolved", amount: 20, color: "#dc2626" },
      { title: "Abandoned", amount: 20, color: "#6b7280" },
    ],
  },
};

export const SixSectionsUneven: Story = {
  args: {
    parts: [
      { title: "Collected", amount: 210, color: "#16a34a" },
      { title: "Assigned", amount: 80, color: "#2563eb" },
      { title: "Unassigned", amount: 35, color: "#f59e0b" },
      { title: "Unresolved", amount: 12, color: "#dc2626" },
      { title: "Abandoned", amount: 6, color: "#6b7280" },
      { title: "Transferred", amount: 3, color: "#9333ea" },
    ],
  },
};

export const WithExplicitTotal: Story = {
  name: "With explicit total (partially filled)",
  args: {
    total: 400,
    parts: [
      { title: "Collected", amount: 150, color: "#16a34a" },
      { title: "Assigned", amount: 70, color: "#2563eb" },
      { title: "Unassigned", amount: 40, color: "#f59e0b" },
      { title: "Unresolved", amount: 20, color: "#dc2626" },
    ],
  },
};

export const DominantSection: Story = {
  args: {
    parts: [
      { title: "Collected", amount: 480, color: "#16a34a" },
      { title: "Assigned", amount: 10, color: "#2563eb" },
      { title: "Unassigned", amount: 5, color: "#f59e0b" },
      { title: "Unresolved", amount: 3, color: "#dc2626" },
      { title: "Abandoned", amount: 2, color: "#6b7280" },
    ],
  },
};
