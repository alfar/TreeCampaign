import type { Meta, StoryObj } from "@storybook/react-vite";
import { MemoryRouter, Outlet, Route, Routes } from "react-router-dom";
import { expect, within } from "storybook/test";

import TeamFaqTab from "./TeamFaqTab";
import type { TeamScreenContext } from "./TeamScreen";
import type { CampaignProgress, CampaignSummary } from "../../shared/api/client";

const campaign: CampaignSummary = {
  id: "11111111-1111-1111-1111-111111111111",
  territoryId: null,
  pricePerTree: 50,
};

const progress: CampaignProgress = {
  unassigned: 40,
  assigned: 60,
  unresolved: 8,
  collected: 120,
  extra: 12,
  total: 240,
};

interface StoryArgs {
  campaign: CampaignSummary | null;
  progress: CampaignProgress | null;
}

// TeamFaqTab reads its data from the parent route's outlet context, so provide it via a layout route.
function renderWithContext({ campaign, progress }: StoryArgs) {
  const context = { campaign, progress } as unknown as TeamScreenContext;
  return (
    <MemoryRouter initialEntries={["/faq"]}>
      <Routes>
        <Route element={<Outlet context={context} />}>
          <Route path="/faq" element={<TeamFaqTab />} />
        </Route>
      </Routes>
    </MemoryRouter>
  );
}

const meta = {
  title: "Teams/TeamFaqTab",
  parameters: { layout: "fullscreen" },
  args: { campaign, progress },
  render: renderWithContext,
} satisfies Meta<StoryArgs>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    await expect(canvas.getByText("Prisen er 50 kr per træ.")).toBeInTheDocument();
    await expect(canvas.getByText("I alt: 240")).toBeInTheDocument();
  },
};

export const DefaultPrice: Story = {
  args: { campaign: null },
  play: async ({ canvasElement }) => {
    await expect(within(canvasElement).getByText("Prisen er 40 kr per træ.")).toBeInTheDocument();
  },
};

export const WithoutProgress: Story = {
  args: { progress: null },
  play: async ({ canvasElement }) => {
    await expect(within(canvasElement).queryByText("Status")).not.toBeInTheDocument();
  },
};

export const EmptyCampaign: Story = {
  args: { progress: { ...progress, unassigned: 0, assigned: 0, unresolved: 0, collected: 0, extra: 0, total: 0 } },
  play: async ({ canvasElement }) => {
    await expect(within(canvasElement).queryByText("Status")).not.toBeInTheDocument();
  },
};
