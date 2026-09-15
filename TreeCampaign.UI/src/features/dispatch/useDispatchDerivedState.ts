import type { Neighborhood } from "../../shared/api/models/neighborhood";
import type { Stop } from "../../shared/api/models/stop";
import { trailerSizeOrder, type Team, type TrailerSize } from "../../shared/api/models/team";

function parseHouseNumber(displayName: string): number {
  const lastToken = displayName.split(",")[0].split(" ").pop() ?? "";
  return parseInt(lastToken, 10) || 0;
}

interface UseDispatchDerivedStateArgs {
  stops: Stop[];
  teams: Team[];
  neighborhoods: Neighborhood[];
  selectedStopIds: Set<string>;
  onlyUnassigned: boolean;
  filter: string;
}

export function useDispatchDerivedState({
  stops,
  teams,
  neighborhoods,
  selectedStopIds,
  onlyUnassigned,
  filter,
}: UseDispatchDerivedStateArgs) {
  const streetSections = neighborhoods.flatMap((n) => n.streetSections);
  const sectionById = new Map(streetSections.map((s) => [s.id, s]));

  const filteredStops =
    onlyUnassigned || filter !== ""
      ? stops.filter(
          (s) =>
            (!onlyUnassigned || s.stopType === "Unassigned") &&
            s.address.displayName
              .toLocaleLowerCase()
              .startsWith(filter.toLocaleLowerCase()),
        )
      : stops;

  const sortedStops = filteredStops.toSorted((a, b) => {
    const sA = sectionById.get(a.address.streetSectionId);
    const sB = sectionById.get(b.address.streetSectionId);
    if (!sA && !sB) return 0;
    if (!sA) return 1;
    if (!sB) return -1;
    if (sA.sortOrder !== sB.sortOrder) return sA.sortOrder - sB.sortOrder;
    const hA = parseHouseNumber(a.address.displayName);
    const hB = parseHouseNumber(b.address.displayName);
    return sA.direction === 0 ? hA - hB : hB - hA;
  });

  const stopsByNeighborhood = neighborhoods
    .toSorted((a, b) =>
      a.streetSections[0].sortOrder < b.streetSections[0].sortOrder ? -1 : 1,
    )
    .map((n) => ({
      neighborhood: n,
      stops: sortedStops.filter(
        (stop) =>
          sectionById.get(stop.address.streetSectionId)?.neighborhoodId ===
          n.id,
      ),
    }))
    .filter((group) => group.stops.length > 0);

  const ungroupedStops = sortedStops.filter(
    (stop) => !sectionById.has(stop.address.streetSectionId),
  );

  const maxTrailerSizeForSelection = Array.from(selectedStopIds)
    .map((stopId) => stops.find((s) => s.id === stopId))
    .map((stop) =>
      stop
        ? sectionById.get(stop.address.streetSectionId)?.maxTrailerSize
        : undefined,
    )
    .filter((size): size is TrailerSize => size !== undefined)
    .reduce<TrailerSize | undefined>(
      (smallest, size) =>
        smallest === undefined ||
        trailerSizeOrder[size] < trailerSizeOrder[smallest]
          ? size
          : smallest,
      undefined,
    );

  const teamExceedsSelection = (team: Team) =>
    team.kind === "Trailer" &&
    !!team.trailerSize &&
    !!maxTrailerSizeForSelection &&
    trailerSizeOrder[team.trailerSize] >
      trailerSizeOrder[maxTrailerSizeForSelection];

  const teamRank = (team: Team) => {
    if (team.kind === "Trailer" && team.isTrailerFull) return 0;
    if (team.kind === "Walking") return 3;
    if (team.status === "OnBreak") return 4;
    const hasAssignedStops = stops.some(
      (s) => s.assignedTeamId === team.id && s.stopType === "Assigned",
    );
    return hasAssignedStops ? 2 : 1;
  };

  const sortedTeams = teams.toSorted((a, b) => teamRank(a) - teamRank(b));
  const teamsById = new Map(teams.map((t) => [t.id, t]));
  const getTeamName = (teamId: string | undefined) =>
    teamId ? teamsById.get(teamId)?.name ?? "Ukendt hold" : undefined;

  return {
    sortedStops,
    stopsByNeighborhood,
    ungroupedStops,
    teamExceedsSelection,
    sortedTeams,
    getTeamName,
  };
}
