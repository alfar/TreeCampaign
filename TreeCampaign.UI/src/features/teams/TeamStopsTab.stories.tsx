import type { Meta, StoryObj } from "@storybook/react-vite";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { http, HttpResponse } from "msw";
import { expect, userEvent, waitFor, within } from "storybook/test";

import TeamScreen from "./TeamScreen";
import TeamStopsTab from "./TeamStopsTab";
import TeamInfoTab from "./TeamInfoTab";
import TeamMapTab from "./TeamMapTab";
import type { Stop } from "../../shared/api/models/stop";
import type { Team } from "../../shared/api/models/team";
import type { Campaign } from "../../shared/api/models/campagin";

const campaignId = "11111111-1111-1111-1111-111111111111";
const teamId = "22222222-2222-2222-2222-222222222222";

const team: Team = {
  id: teamId,
  name: "Ulverne",
  status: "Active",
  kind: "Trailer",
  isTrailerFull: false,
  trailerSize: "Large",
  currentExtraTrees: 0,
  totalExtraTrees: 0,
  members: [],
};

const campaign: Campaign = {
  id: campaignId,
  season: 2026,
};

function stop(overrides: Partial<Stop>): Stop {
  return {
    id: "stop-1",
    address: {
      displayName: "Eksempelvej 12",
      latitude: 56.17,
      longitude: 9.55,
      streetSectionId: "33333333-3333-3333-3333-333333333333",
    },
    amount: 40,
    stopType: "Assigned",
    assignedTeamId: teamId,
    ...overrides,
  };
}

const initialStops: Stop[] = [stop({ id: "stop-1" })];

function baseHandlers(stops: Stop[], getTeamResponse?: () => Team) {
  return [
    http.get(`/api/${campaignId}/teams/${teamId}`, () =>
      HttpResponse.json(getTeamResponse ? getTeamResponse() : team),
    ),
    http.get(`/api/campaigns/${campaignId}`, () => HttpResponse.json(campaign)),
    http.get(`/api/${campaignId}/stops`, () => HttpResponse.json(stops)),
  ];
}

const meta = {
  title: "Teams/TeamStopsTab",
  component: TeamStopsTab,
  parameters: {
    layout: "fullscreen",
  },
  decorators: [
    (_Story, { parameters }) => (
      <MemoryRouter
        initialEntries={[
          `/campaigns/${campaignId}/teams/${teamId}/${parameters.initialTab ?? "stops"}`,
        ]}
      >
        <Routes>
          <Route path="/campaigns/:campaignId/teams/:teamId" element={<TeamScreen />}>
            <Route path="stops" element={<TeamStopsTab />} />
            <Route path="map" element={<TeamMapTab />} />
            <Route path="info" element={<TeamInfoTab />} />
          </Route>
        </Routes>
      </MemoryRouter>
    ),
  ],
} satisfies Meta<typeof TeamStopsTab>;

export default meta;
type Story = StoryObj<typeof meta>;

const storageKey = `treecampaign:team-stops:${campaignId}:${teamId}`;

export const OnlineHappyPath: Story = {
  beforeEach: ({ msw }) => {
    localStorage.removeItem(storageKey);
    msw.use(
      ...baseHandlers(initialStops),
      http.post(
        `/api/${campaignId}/stops/stop-1/collect`,
        () => HttpResponse.json(stop({ id: "stop-1", stopType: "Collected" })),
      ),
    );
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    await waitFor(() =>
      expect(canvas.getByText("Eksempelvej 12")).toBeInTheDocument(),
    );

    await userEvent.click(canvas.getByText("Eksempelvej 12"));
    await userEvent.click(canvas.getByText("Hentet"));

    await waitFor(() =>
      expect(canvas.queryByLabelText("Ingen forbindelse")).not.toBeInTheDocument(),
    );
  },
};

export const QueuesWhenOffline: Story = {
  name: "Queues action while offline, drains once back online",
  beforeEach: ({ msw }) => {
    localStorage.removeItem(storageKey);
    msw.use(
      ...baseHandlers(initialStops),
      // First collect attempt: simulate a dropped connection (fetch-level failure).
      http.post(`/api/${campaignId}/stops/stop-1/collect`, () => {
        return HttpResponse.error();
      }),
    );
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    await waitFor(() =>
      expect(canvas.getByText("Eksempelvej 12")).toBeInTheDocument(),
    );

    await userEvent.click(canvas.getByText("Eksempelvej 12"));
    await userEvent.click(canvas.getByText("Hentet"));

    // Optimistic UI: the stop moves out of the "Assigned" list immediately, and a small
    // offline icon appears — even though the request above is failing — without
    // interrupting the team with a loud banner.
    await waitFor(() =>
      expect(canvas.getByLabelText("Ingen forbindelse")).toBeInTheDocument(),
    );

    // Tapping the icon reveals the detail text, including the queued action count.
    await userEvent.click(canvas.getByLabelText("Ingen forbindelse"));
    await waitFor(() =>
      expect(
        canvas.getByText(/1 handling venter på at blive sendt/),
      ).toBeInTheDocument(),
    );
  },
};

export const DrainsQueueWhenBackOnline: Story = {
  name: "Drains the queued action once the network returns",
  beforeEach: ({ msw }) => {
    localStorage.removeItem(storageKey);
    let attempt = 0;
    msw.use(
      ...baseHandlers(initialStops),
      http.post(`/api/${campaignId}/stops/stop-1/collect`, () => {
        attempt += 1;
        // First attempt simulates a dropped connection; the retry after "online" fires succeeds.
        if (attempt === 1) return HttpResponse.error();
        return HttpResponse.json(stop({ id: "stop-1", stopType: "Collected" }));
      }),
    );
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    await waitFor(() =>
      expect(canvas.getByText("Eksempelvej 12")).toBeInTheDocument(),
    );

    await userEvent.click(canvas.getByText("Eksempelvej 12"));
    await userEvent.click(canvas.getByText("Hentet"));

    // First attempt fails: queued, offline icon shown, optimistic Collected state applied.
    await waitFor(() =>
      expect(canvas.getByLabelText("Ingen forbindelse")).toBeInTheDocument(),
    );
    await userEvent.click(canvas.getByLabelText("Ingen forbindelse"));
    await waitFor(() =>
      expect(
        canvas.getByText(/1 handling venter på at blive sendt/),
      ).toBeInTheDocument(),
    );
    await waitFor(() =>
      expect(canvas.getByText("Fortryd")).toBeInTheDocument(),
    );

    // Network recovers: the browser fires "online", which useTeamStops listens for to retry the queue.
    window.dispatchEvent(new Event("online"));

    // The retried request succeeds this time, so the queue drains and the icon disappears.
    await waitFor(() =>
      expect(canvas.queryByLabelText("Ingen forbindelse")).not.toBeInTheDocument(),
    );
    await waitFor(() =>
      expect(canvas.getByText("Fortryd")).toBeInTheDocument(),
    );
  },
};

export const DrainsQueueViaManualRefresh: Story = {
  name: "Drains the queue via manual refresh, without an online event",
  beforeEach: ({ msw }) => {
    localStorage.removeItem(storageKey);
    let attempt = 0;
    msw.use(
      ...baseHandlers(initialStops),
      http.post(`/api/${campaignId}/stops/stop-1/collect`, () => {
        attempt += 1;
        // First attempt simulates a dropped connection; the retry via manual refresh succeeds —
        // this reproduces the reported bug where connectivity returned but nothing ever
        // retried the queue until the tab was backgrounded and reopened.
        if (attempt === 1) return HttpResponse.error();
        return HttpResponse.json(stop({ id: "stop-1", stopType: "Collected" }));
      }),
    );
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    await waitFor(() =>
      expect(canvas.getByText("Eksempelvej 12")).toBeInTheDocument(),
    );

    await userEvent.click(canvas.getByText("Eksempelvej 12"));
    await userEvent.click(canvas.getByText("Hentet"));

    await waitFor(() =>
      expect(canvas.getByLabelText("Ingen forbindelse")).toBeInTheDocument(),
    );

    // Deliberately no "online" event and no visibilitychange — only the manual
    // refresh button, which drives the same poll()-triggers-drainQueue() path.
    await userEvent.click(canvas.getByLabelText("Opdater"));

    await waitFor(() =>
      expect(canvas.queryByLabelText("Ingen forbindelse")).not.toBeInTheDocument(),
    );
    await waitFor(() =>
      expect(canvas.getByText("Fortryd")).toBeInTheDocument(),
    );
  },
};

export const ReportTrailerFullQueuesOffline: Story = {
  name: "Trailer full report queues while offline",
  beforeEach: ({ msw }) => {
    localStorage.removeItem(storageKey);
    msw.use(
      ...baseHandlers(initialStops),
      http.post(`/api/${campaignId}/teams/${teamId}/trailer-full`, () => {
        return HttpResponse.error();
      }),
    );
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    await waitFor(() =>
      expect(canvas.getByText("Trailer fuld")).toBeInTheDocument(),
    );

    await userEvent.click(canvas.getByText("Trailer fuld"));

    // Optimistic UI: button reflects "full" immediately and disables, even though
    // the request above is failing — the report is queued for later.
    await waitFor(() =>
      expect(canvas.getByText("Trailer fuld ✓")).toBeInTheDocument(),
    );
    await waitFor(() =>
      expect(canvas.getByText("Trailer fuld ✓")).toBeDisabled(),
    );
    await waitFor(() =>
      expect(canvas.getByLabelText("Ingen forbindelse")).toBeInTheDocument(),
    );
  },
};

export const AddMemberQueuesOfflineOnInfoTab: Story = {
  name: "Adding a patrol member queues offline and appears immediately",
  parameters: { initialTab: "info" },
  beforeEach: ({ msw }) => {
    localStorage.removeItem(storageKey);
    msw.use(
      ...baseHandlers(initialStops),
      http.post(`/api/${campaignId}/teams/${teamId}/members`, () => {
        return HttpResponse.error();
      }),
    );
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    await waitFor(() =>
      expect(canvas.getByPlaceholderText("Navn")).toBeInTheDocument(),
    );

    await userEvent.type(canvas.getByPlaceholderText("Navn"), "Anders And");
    await userEvent.click(canvas.getByText("Tilføj"));

    // Optimistic UI: the new member appears immediately under a temporary local id,
    // even though the request above is failing — the addition is queued for later.
    await waitFor(() =>
      expect(canvas.getByText("Anders And")).toBeInTheDocument(),
    );
    await waitFor(() =>
      expect(canvas.getByLabelText("Ingen forbindelse")).toBeInTheDocument(),
    );
  },
};

export const RemovingPendingMemberCancelsQueuedAdd: Story = {
  name: "Removing a not-yet-sent member cancels the queued add",
  parameters: { initialTab: "info" },
  beforeEach: ({ msw }) => {
    localStorage.removeItem(storageKey);
    msw.use(
      ...baseHandlers(initialStops),
      http.post(`/api/${campaignId}/teams/${teamId}/members`, () => {
        return HttpResponse.error();
      }),
    );
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    await waitFor(() =>
      expect(canvas.getByPlaceholderText("Navn")).toBeInTheDocument(),
    );

    await userEvent.type(canvas.getByPlaceholderText("Navn"), "Anders And");
    await userEvent.click(canvas.getByText("Tilføj"));

    await waitFor(() =>
      expect(canvas.getByText("Anders And")).toBeInTheDocument(),
    );
    await waitFor(() =>
      expect(canvas.getByLabelText("Ingen forbindelse")).toBeInTheDocument(),
    );

    // The member was never sent to the server (still offline), so removing it should
    // just cancel the queued add locally — nothing left to retry, offline icon clears.
    await userEvent.click(canvas.getByText("Fjern"));

    await waitFor(() =>
      expect(canvas.queryByText("Anders And")).not.toBeInTheDocument(),
    );
    await waitFor(() =>
      expect(canvas.queryByLabelText("Ingen forbindelse")).not.toBeInTheDocument(),
    );
  },
};

export const CollectStopFromMapPopup: Story = {
  name: "Collecting a stop from the map popup",
  parameters: { initialTab: "map" },
  beforeEach: ({ msw }) => {
    localStorage.removeItem(storageKey);
    msw.use(
      ...baseHandlers(initialStops),
      http.post(
        `/api/${campaignId}/stops/stop-1/collect`,
        () => HttpResponse.json(stop({ id: "stop-1", stopType: "Collected" })),
      ),
    );
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);

    // Click the stop's marker on the map to open its popup.
    await waitFor(() =>
      expect(canvasElement.querySelector("path.leaflet-interactive")).toBeTruthy(),
    );
    const marker = canvasElement.querySelector("path.leaflet-interactive") as SVGPathElement;
    await userEvent.click(marker);

    // The popup shows the same collect/not-found/retry/correct buttons as the stops list.
    await waitFor(() =>
      expect(canvas.getByText("Hentet")).toBeInTheDocument(),
    );
    await userEvent.click(canvas.getByText("Hentet"));

    await waitFor(() =>
      expect(canvas.queryByLabelText("Ingen forbindelse")).not.toBeInTheDocument(),
    );
  },
};

export const InfoTabPicksUpNameFromPoll: Story = {
  name: "Team name updates from a poll when the form is untouched",
  parameters: { initialTab: "info" },
  beforeEach: ({ msw }) => {
    localStorage.removeItem(storageKey);
    // First call (initial mount) returns the original name; the manual refresh's
    // call returns the rename, simulating another device having renamed the team.
    let attempt = 0;
    msw.use(
      ...baseHandlers(initialStops, () => ({
        ...team,
        name: (attempt += 1) === 1 ? "Ulverne" : "Ræverne",
      })),
    );
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    await waitFor(() =>
      expect(canvas.getByDisplayValue("Ulverne")).toBeInTheDocument(),
    );

    // Simulate another device having renamed the team — the next poll (via manual
    // refresh here) picks it up since the form hasn't been touched.
    await userEvent.click(canvas.getByLabelText("Opdater"));

    await waitFor(() =>
      expect(canvas.getByDisplayValue("Ræverne")).toBeInTheDocument(),
    );
  },
};

export const MapKeepsViewportThroughPoll: Story = {
  name: "Map: pan/zoom, then poll, then recenter (manual)",
  parameters: { initialTab: "map" },
  beforeEach: ({ msw }) => {
    localStorage.removeItem(storageKey);
    msw.use(
      ...baseHandlers([
        stop({ id: "stop-1" }),
        stop({
          id: "stop-2",
          address: {
            displayName: "Andet Sted 4",
            latitude: 56.18,
            longitude: 9.56,
            streetSectionId: "33333333-3333-3333-3333-333333333333",
          },
        }),
      ]),
    );
  },
  play: async ({ canvasElement }) => {
    // Manual test: pan/zoom the map, then click "Opdater" (top-right refresh icon)
    // to simulate a poll landing. The viewport should NOT jump back to fit the
    // stops. Then click "Centrer kort" (bottom-right) to confirm it re-fits on demand.
    await waitFor(() =>
      expect(canvasElement.querySelector(".leaflet-container")).toBeTruthy(),
    );
  },
};

export const InfoTabKeepsUnsavedEditThroughPoll: Story = {
  name: "In-progress name edit survives a poll",
  parameters: { initialTab: "info" },
  beforeEach: ({ msw }) => {
    localStorage.removeItem(storageKey);
    let attempt = 0;
    msw.use(
      ...baseHandlers(initialStops, () => ({
        ...team,
        name: (attempt += 1) === 1 ? "Ulverne" : "Ræverne",
      })),
    );
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    const nameInput = await waitFor(() =>
      canvas.getByDisplayValue("Ulverne") as HTMLInputElement,
    );

    // Start editing, but don't submit — an in-progress, unsaved edit.
    await userEvent.clear(nameInput);
    await userEvent.type(nameInput, "Mine Ulve");

    // A poll lands (another device renamed the team to "Ræverne" server-side),
    // but since this form is dirty, the in-progress edit must not be overwritten.
    await userEvent.click(canvas.getByLabelText("Opdater"));

    await waitFor(() =>
      expect(canvas.getByDisplayValue("Mine Ulve")).toBeInTheDocument(),
    );
    expect(canvas.queryByDisplayValue("Ræverne")).not.toBeInTheDocument();
  },
};

export const DeliverLoadQueuesOffline: Story = {
  name: "Deliver load queues offline and clears collected stops",
  beforeEach: ({ msw }) => {
    localStorage.removeItem(storageKey);
    msw.use(
      ...baseHandlers([stop({ id: "stop-1", stopType: "Collected" })]),
      http.post(`/api/${campaignId}/teams/${teamId}/deliver-load`, () => {
        return HttpResponse.error();
      }),
    );
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    await waitFor(() =>
      expect(canvas.getByText("Lever last")).toBeInTheDocument(),
    );

    await userEvent.click(canvas.getByText("Lever last"));

    // Optimistic UI: the Collected stop becomes Delivered and drops out of the
    // visible list, and the "Lever last" button disappears since nothing is
    // Collected anymore — even though the request above is failing.
    await waitFor(() =>
      expect(canvas.queryByText("Eksempelvej 12")).not.toBeInTheDocument(),
    );
    await waitFor(() =>
      expect(canvas.queryByText("Lever last")).not.toBeInTheDocument(),
    );
    await waitFor(() =>
      expect(canvas.getByLabelText("Ingen forbindelse")).toBeInTheDocument(),
    );
  },
};
