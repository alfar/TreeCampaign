import type { Campaign } from "./models/campagin";
import type { Neighborhood } from "./models/neighborhood";
import type { Order } from "./models/order";
import type { PaymentImportSummary } from "./models/paymentImport";
import type { Street } from "./models/street";
import type { Stop } from "./models/stop";
import type { Team, TeamKind, TrailerSize } from "./models/team";
import type { Territory } from "./models/territory";

export async function getOrders(campaignId: string): Promise<Order[]> {
  const res = await fetch(`/api/campaigns/${campaignId}/orders`);
  return res.json();
}

export async function createOrder(
  campaignId: string,
  data: { orderDate: string; senderName: string; senderPhoneNumber?: string; amount: number; message: string }
): Promise<Response> {
  return fetch(`/api/campaigns/${campaignId}/orders`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
}

export async function importPayments(campaignId: string, file: File): Promise<PaymentImportSummary> {
  const formData = new FormData();
  formData.append('file', file);

  const res = await fetch(`/api/campaigns/${campaignId}/orders/import`, {
    method: 'POST',
    body: formData,
  });
  return res.json();
}

export async function washOrder(
  campaignId: string,
  orderId: string,
  data: { streetId: string; houseNumber: string }
): Promise<Response> {
  return fetch(`/api/campaigns/${campaignId}/orders/${orderId}/wash`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
}

export async function transferOrder(
  campaignId: string,
  orderId: string,
  territoryId: string
): Promise<Response> {
  return fetch(`/api/campaigns/${campaignId}/orders/${orderId}/transfer`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ territoryId }),
  });
}

export async function undoTransferOrder(campaignId: string, orderId: string): Promise<Response> {
  return fetch(`/api/campaigns/${campaignId}/orders/${orderId}/transfer`, {
    method: 'DELETE',
  });
}

export async function settleOrder(campaignId: string, orderId: string): Promise<Response> {
  return fetch(`/api/campaigns/${campaignId}/orders/${orderId}/settle`, {
    method: 'POST',
  });
}

export async function getStreetsByZipCode(zipCode: string): Promise<Street[]> {
  const res = await fetch(`/api/streets/${zipCode}`);
  return res.json();
}

export async function createStreet(name: string, zipCode: string): Promise<Street> {
  const res = await fetch('/api/streets/', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name, zipCode }),
  });
  return res.json();
}

export async function getCampaigns() : Promise<Campaign[]> {
    const res = await fetch('/api/campaigns');
    return res.json();
}

export async function getCampaign(campaignId: string): Promise<Campaign> {
  const res = await fetch(`/api/campaigns/${campaignId}`);
  return res.json();
}

export async function createCampaign(year: number, territoryId?: string): Promise<Campaign> {
  const res = await fetch('/api/campaigns', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ season: year, territoryId: territoryId ?? null }),
  });
  return res.json();
}

export async function updateCampaign(campaignId: string, year: number, territoryId?: string): Promise<Campaign> {
  const res = await fetch(`/api/campaigns/${campaignId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ season: year, territoryId: territoryId ?? null }),
  });
  return res.json();
}

export async function getTerritories(): Promise<Territory[]> {
  const res = await fetch('/api/Territories');
  return res.json();
}

export async function getTerritory(territoryId: string): Promise<Territory> {
  const res = await fetch(`/api/territories/${territoryId}`);
  return res.json();
}

export async function createTerritory(name: string, defaultZipCode: string): Promise<Territory> {
  const res = await fetch('/api/Territories', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name, defaultZipCode }),
  });
  return res.json();
}

export async function getNeighborhoods(territoryId: string): Promise<Neighborhood[]> {
  const res = await fetch(`/api/Territories/${territoryId}/neighborhoods`);
  return res.json();
}

export async function createNeighborhood(territoryId: string, name: string): Promise<Neighborhood> {
  const res = await fetch(`/api/Territories/${territoryId}/neighborhoods`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name }),
  });
  return res.json();
}

export async function createStreetSection(
  territoryId: string,
  neighborhoodId: string,
  streetId: string,
  sortOrder: number = 0,
  fromHouseNumber: string | null = null,
  toHouseNumber: string | null = null,
  direction: number = 0,
): Promise<Response> {
  return fetch(`/api/Territories/${territoryId}/neighborhoods/${neighborhoodId}/street-sections`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ streetId, sortOrder, fromHouseNumber, toHouseNumber, direction }),
  });
}

export async function updateStreetSection(
  territoryId: string,
  neighborhoodId: string,
  streetSectionId: string,
  sortOrder: number = 0,
  fromHouseNumber: string | null = null,
  toHouseNumber: string | null = null,
  direction: number = 0,
): Promise<Response> {
  return fetch(`/api/Territories/${territoryId}/neighborhoods/${neighborhoodId}/street-sections/${streetSectionId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ sortOrder, fromHouseNumber, toHouseNumber, direction }),
  });
}

export async function deleteStreetSection(
  territoryId: string,
  neighborhoodId: string,
  streetSectionId: string,
): Promise<Response> {
  return fetch(`/api/Territories/${territoryId}/neighborhoods/${neighborhoodId}/street-sections/${streetSectionId}`, {
    method: 'DELETE',
  });
}

export async function getStops(campaignId: string) : Promise<Stop[]> {
  const res = await fetch(`/api/${campaignId}/stops`);
  return res.json();
}

export async function getStopsForTeam(campaignId: string,teamId: string) : Promise<Stop[]> {
  const res = await fetch(`/api/${campaignId}/stops?teamId=${teamId}`);
  return res.json();
}

export async function getTeams(campaignId: string) : Promise<Team[]> {
  const res = await fetch(`/api/${campaignId}/teams`);
  return res.json();
}

export async function createTeam(campaignId: string, name: string, kind: TeamKind, trailerSize?: TrailerSize): Promise<Team> {
  const res = await fetch(`/api/${campaignId}/teams`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name, kind, trailerSize }),
  });
  return res.json();
}

export async function updateTeam(campaignId: string, teamId: string, name: string, trailerSize?: TrailerSize): Promise<Team> {
  const res = await fetch(`/api/${campaignId}/teams/${teamId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name, trailerSize }),
  });
  return res.json();
}

export async function assignStopToTeam(campaignId: string, stopId: string, teamId: string) : Promise<Stop> {
  return await fetch(`/api/${campaignId}/stops/${stopId}/assign`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ teamId })
  }).then(res => res.json());
}

export async function unassignStop(campaignId: string, stopId: string) : Promise<Stop> {
  return await fetch(`/api/${campaignId}/stops/${stopId}/unassign`, {
    method: 'POST'
  }).then(res => res.json());
}

export async function collectStop(campaignId: string, stopId: string) : Promise<Stop> {
  return await fetch(`/api/${campaignId}/stops/${stopId}/collect`, {
    method: 'POST'
  }).then(res => res.json());
}

export async function correctStop(campaignId: string, stopId: string) : Promise<Stop> {
  return await fetch(`/api/${campaignId}/stops/${stopId}/correct`, {
    method: 'POST'
  }).then(res => res.json());
}

export async function retryStop(campaignId: string, stopId: string) : Promise<Stop> {
  return await fetch(`/api/${campaignId}/stops/${stopId}/retry`, {
    method: 'POST'
  }).then(res => res.json());
}

export async function markStopUnresolved(campaignId: string, stopId: string, reason: string = 'Ikke fundet') : Promise<Stop> {
  return await fetch(`/api/${campaignId}/stops/${stopId}/unresolved`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ reason })
  }).then(res => res.json());
}

export async function reopenStop(campaignId: string, stopId: string) : Promise<Stop> {
  return await fetch(`/api/${campaignId}/stops/${stopId}/reopen`, {
    method: 'POST'
  }).then(res => res.json());
}

export async function deliverLoad(campaignId: string, teamId: string): Promise<{ deliveredCount: number }> {
  const res = await fetch(`/api/${campaignId}/teams/${teamId}/deliver-load`, { method: 'POST' });
  return res.json();
}

export async function sendTeamOnBreak(campaignId: string, teamId: string): Promise<Team> {
  const res = await fetch(`/api/${campaignId}/teams/${teamId}/break`, { method: 'POST' });
  return res.json();
}

export async function reportTrailerFull(campaignId: string, teamId: string): Promise<Team> {
  const res = await fetch(`/api/${campaignId}/teams/${teamId}/trailer-full`, { method: 'POST' });
  return res.json();
}

export async function clearTrailerFull(campaignId: string, teamId: string): Promise<Team> {
  const res = await fetch(`/api/${campaignId}/teams/${teamId}/trailer-full`, { method: 'DELETE' });
  return res.json();
}

export async function addTeamMember(
  campaignId: string,
  teamId: string,
  name: string,
  phoneNumber?: string,
  scoutRelativeName?: string,
): Promise<Team> {
  const res = await fetch(`/api/${campaignId}/teams/${teamId}/members`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name, scoutRelativeName, phoneNumber }),
  });
  return res.json();
}

export async function removeTeamMember(
  campaignId: string,
  teamId: string,
  memberId: string,
): Promise<Team> {
  const res = await fetch(`/api/${campaignId}/teams/${teamId}/members/${memberId}`, {
    method: 'DELETE',
  });
  return res.json();
}

export async function requestPickup(
  campaignId: string,
  streetId: string,
  houseNumber: string,
  treeCount: number,
): Promise<Stop> {
  const res = await fetch(`/api/${campaignId}/stops/pickup-request`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ streetId, houseNumber, treeCount }),
  });
  return res.json();
}
