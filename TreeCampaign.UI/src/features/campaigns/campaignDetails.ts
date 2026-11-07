export interface CampaignDetailsValues {
  title: string;
  collectionDate: string;
  pricePerTree: string;
  lastOrderDateTime: string;
}

export const emptyCampaignDetails: CampaignDetailsValues = {
  title: "",
  collectionDate: "",
  pricePerTree: "",
  lastOrderDateTime: "",
};

const pad = (n: number) => String(n).padStart(2, "0");

// <input type="datetime-local"> wants local "YYYY-MM-DDTHH:mm", the API uses ISO offset timestamps.
export function toLocalInputValue(iso?: string | null): string {
  if (!iso) return "";
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return "";
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

export function fromLocalInputValue(value: string): string | null {
  if (!value) return null;
  const d = new Date(value);
  return Number.isNaN(d.getTime()) ? null : d.toISOString();
}

export function pricePerTreeIsValid(value: string): boolean {
  return value === "" || Number(value) > 0;
}
