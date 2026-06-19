export interface StreetSection {
  id: string;
  neighborhoodId: string;
  streetId: string;
  startHouseNumber: string | null;
  endHouseNumber: string | null;
  sortOrder: number;
  direction: number; // 0 = Ascending, 1 = Descending
}
