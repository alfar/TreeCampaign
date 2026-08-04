import type { TrailerSize } from "./team";

export interface StreetSection {
  id: string;
  neighborhoodId: string;
  streetId: string;
  evenStartHouseNumber: string | null;
  evenEndHouseNumber: string | null;
  oddStartHouseNumber: string | null;
  oddEndHouseNumber: string | null;
  sortOrder: number;
  direction: number; // 0 = Ascending, 1 = Descending
  maxTrailerSize: TrailerSize;
}
