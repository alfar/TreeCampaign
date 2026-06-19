import type { StreetSection } from "./streetSection";

export interface Neighborhood {
  id: string;
  name: string;
  streetSections: StreetSection[];
}
