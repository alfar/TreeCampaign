import type { Campaign } from "./models/campagin";
import type { Stop } from "./models/stop";
import type { Team } from "./models/team";

export async function getCampaigns() : Promise<Campaign[]> {
    const res = await fetch('/api/campaigns');
    return res.json();
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
