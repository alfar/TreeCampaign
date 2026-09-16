import type { Meta, StoryObj } from "@storybook/react-vite";

import StopListPanel from "./StopListPanel";
import type { Stop } from "../../shared/api/models/stop";
import type { Neighborhood } from "../../shared/api/models/neighborhood";

const meta = {
  title: "Dispatch/StopListPanel",
  component: StopListPanel,
  parameters: {
    layout: "fullscreen",
  },
} satisfies Meta<typeof StopListPanel>;

export default meta;
type Story = StoryObj<typeof meta>;

const getTeamName = (id: string | undefined) => (id ? `Team ${id}` : undefined);

const streetSection = {
  id: "00000000-0000-0000-0000-000000000010",
  neighborhoodId: "00000000-0000-0000-0000-000000000020",
  streetId: "00000000-0000-0000-0000-000000000030",
  evenStartHouseNumber: null,
  evenEndHouseNumber: null,
  oddStartHouseNumber: null,
  oddEndHouseNumber: null,
  sortOrder: 0,
  direction: 0,
  maxTrailerSize: "Boogie" as const,
};

const neighborhoods: Neighborhood[] = [
  {
    id: "00000000-0000-0000-0000-000000000020",
    name: "Nordbyen",
    streetSections: [streetSection],
  },
];

const stops: Stop[] = [
  {
    id: "00000000-0000-0000-0000-000000000001",
    address: {
      streetSectionId: streetSection.id,
      displayName: "Vesterbakken 28",
      latitude: 55.6761,
      longitude: 12.5683,
    },
    amount: 2,
    stopType: "Unassigned",
    assignedTeamId: undefined,
  },
  {
    id: "00000000-0000-0000-0000-000000000002",
    address: {
      streetSectionId: streetSection.id,
      displayName: "Vesterbakken 30",
      latitude: 55.6761,
      longitude: 12.5683,
    },
    amount: 1,
    stopType: "Assigned",
    assignedTeamId: "00000000-0000-0000-0000-000000000040",
  },
];

const ungroupedStop: Stop = {
  id: "00000000-0000-0000-0000-000000000003",
  address: {
    streetSectionId: "00000000-0000-0000-0000-000000000099",
    displayName: "Ukendt Vej 1",
    latitude: 55.6761,
    longitude: 12.5683,
  },
  amount: 1,
  stopType: "Unresolved",
  assignedTeamId: undefined,
};

export const WithStops: Story = {
  args: {
    campaignId: "00000000-0000-0000-0000-000000000000",
    selectedStopTypes: new Set(["Unassigned", "Assigned"]),
    setSelectedStopTypes: () => {},
    filter: "",
    setFilter: () => {},
    sortedStops: stops,
    stopsByNeighborhood: [{ neighborhood: neighborhoods[0], stops }],
    ungroupedStops: [],
    getTeamName,
    selectedStopIds: new Set<string>(),
    toggleStop: () => {},
  },
};

export const WithUngroupedStop: Story = {
  args: {
    campaignId: "00000000-0000-0000-0000-000000000000",
    selectedStopTypes: new Set(["Unresolved"]),
    setSelectedStopTypes: () => {},
    filter: "",
    setFilter: () => {},
    sortedStops: [ungroupedStop],
    stopsByNeighborhood: [],
    ungroupedStops: [ungroupedStop],
    getTeamName,
    selectedStopIds: new Set<string>(),
    toggleStop: () => {},
  },
};

export const Empty: Story = {
  args: {
    campaignId: "00000000-0000-0000-0000-000000000000",
    selectedStopTypes: new Set(["Unassigned"]),
    setSelectedStopTypes: () => {},
    filter: "",
    setFilter: () => {},
    sortedStops: [],
    stopsByNeighborhood: [],
    ungroupedStops: [],
    getTeamName,
    selectedStopIds: new Set<string>(),
    toggleStop: () => {},
  },
};
export const SelectionCount: Story = {
  args: {
    campaignId: "00000000-0000-0000-0000-000000000000",
    selectedStopTypes: new Set(["Unassigned", "Assigned"]),
    setSelectedStopTypes: () => {},
    filter: "",
    setFilter: () => {},
    sortedStops: stops,
    stopsByNeighborhood: [{ neighborhood: neighborhoods[0], stops }],
    ungroupedStops: [],
    getTeamName,
    selectedStopIds: new Set<string>(["00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002"]),
    toggleStop: () => {},
  },
};

