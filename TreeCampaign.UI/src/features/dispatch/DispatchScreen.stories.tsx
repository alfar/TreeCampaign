import type { Meta, StoryObj } from "@storybook/react-vite";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { http, HttpResponse } from "msw";

import DispatchScreen from "./DispatchScreen";
import { AuthProvider } from "../../app/AuthContext";
import type { Campaign } from "../../shared/api/models/campagin";
import type { Neighborhood } from "../../shared/api/models/neighborhood";
import type { Stop, StopType } from "../../shared/api/models/stop";
import type { Team, TrailerSize } from "../../shared/api/models/team";

const campaignId = "11111111-1111-1111-1111-111111111111";
const territoryId = "22222222-2222-2222-2222-222222222222";

const campaign: Campaign = { id: campaignId, season: 2026, territoryId };

const sizes: TrailerSize[] = ["Large", "Boogie", "Small", "Runner"];
const neighborhoodNames = ["Nordbyen", "Vesterbro", "Skovvejskvarteret", "Havnen"];

const neighborhoods: Neighborhood[] = neighborhoodNames.map((name, n) => ({
  id: `nb-${n}`,
  name,
  streetSections: [0, 1].map((s) => ({
    id: `sec-${n}-${s}`,
    neighborhoodId: `nb-${n}`,
    streetId: `street-${n}-${s}`,
    evenStartHouseNumber: null,
    evenEndHouseNumber: null,
    oddStartHouseNumber: null,
    oddEndHouseNumber: null,
    sortOrder: n * 10 + s,
    direction: 0,
    maxTrailerSize: sizes[(n + s) % sizes.length],
  })),
}));

const streets = ["Vesterbakken", "Skovvej", "Havnegade", "Parkallé", "Kirkestien"];
const stopTypes: StopType[] = [
  "Unassigned",
  "Unassigned",
  "Unassigned",
  "Assigned",
  "Unresolved",
  "Collected",
];

const teams: Team[] = [
  {
    id: "team-1",
    name: "Ulverne",
    status: "Active",
    kind: "Trailer",
    isTrailerFull: true,
    trailerSize: "Large",
    currentExtraTrees: 3,
    totalExtraTrees: 5,
    members: [{ id: "m1", name: "Lars Hansen", phoneNumber: "12345678" }],
  },
  {
    id: "team-2",
    name: "Bæverne",
    status: "Active",
    kind: "Trailer",
    isTrailerFull: false,
    trailerSize: "Small",
    currentExtraTrees: 0,
    totalExtraTrees: 0,
    members: [],
  },
  {
    id: "team-3",
    name: "Egernerne",
    status: "Active",
    kind: "Trailer",
    isTrailerFull: false,
    trailerSize: "Boogie",
    currentExtraTrees: 0,
    totalExtraTrees: 2,
    members: [],
  },
  {
    id: "team-4",
    name: "Løberne",
    status: "Active",
    kind: "Walking",
    isTrailerFull: null,
    trailerSize: null,
    currentExtraTrees: 0,
    totalExtraTrees: 0,
    members: [],
  },
  {
    id: "team-5",
    name: "Rævene",
    status: "OnBreak",
    kind: "Trailer",
    isTrailerFull: false,
    trailerSize: "Large",
    currentExtraTrees: 0,
    totalExtraTrees: 0,
    members: [],
  },
];

const stops: Stop[] = neighborhoods.flatMap((nb, n) =>
  nb.streetSections.flatMap((section, s) =>
    Array.from({ length: 5 }, (_, i): Stop => {
      const stopType = stopTypes[(n + s + i) % stopTypes.length];
      const assigned = stopType === "Assigned" || stopType === "Collected";
      return {
        id: `stop-${n}-${s}-${i}`,
        address: {
          streetSectionId: section.id,
          displayName: `${streets[(n + s) % streets.length]} ${i * 2 + 1}, 8000 Aarhus`,
          latitude: 56.15,
          longitude: 10.2,
        },
        amount: (i % 3) + 1,
        stopType,
        assignedTeamId: assigned ? teams[(n + i) % 3].id : undefined,
      };
    }),
  ),
);

const meta = {
  title: "Dispatch/DispatchScreen",
  component: DispatchScreen,
  parameters: {
    layout: "fullscreen",
    msw: {
      handlers: [
        http.get("/api/auth/me", () =>
          HttpResponse.json({
            userId: "user-1",
            email: "dispatcher@example.com",
            displayName: "Dispatcher",
            scoutGroupId: "group-1",
            isPlatformAdmin: false,
          }),
        ),
        http.get(`/api/campaigns/${campaignId}`, () =>
          HttpResponse.json(campaign),
        ),
        http.get(`/api/${campaignId}/stops`, () => HttpResponse.json(stops)),
        http.get(`/api/${campaignId}/teams`, () => HttpResponse.json(teams)),
        http.get(`/api/Territories/${territoryId}/neighborhoods`, () =>
          HttpResponse.json(neighborhoods),
        ),
      ],
    },
  },
  decorators: [
    (Story) => (
      <AuthProvider>
        <MemoryRouter initialEntries={[`/campaigns/${campaignId}/dispatch`]}>
          <Routes>
            <Route path="/campaigns/:campaignId/dispatch" element={<Story />} />
          </Routes>
        </MemoryRouter>
      </AuthProvider>
    ),
  ],
} satisfies Meta<typeof DispatchScreen>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {};
