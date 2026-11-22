import { useParams } from "react-router-dom";
import { assignStopToTeam } from "../../shared/api/client";
import { useState } from "react";
import type { StopType } from "../../shared/api/models/stop";
import type { Team, TrailerSize } from "../../shared/api/models/team";
import NavigationPage from "../../shared/components/NavigationPage";
import { useDispatchData } from "./useDispatchData";
import { useDispatchDerivedState } from "./useDispatchDerivedState";
import { allMaxTrailerSizes } from "./StopMaxTrailerSizeFilter";
import StopListPanel from "./StopListPanel";
import TeamListPanel from "./TeamListPanel";

export default function DispatchScreen() {
  const params = useParams();
  const campaignId = params.campaignId!;
  const { stops, teams, neighborhoods, updateStop, updateTeam } =
    useDispatchData(campaignId);
  const [selectedStopIds, setSelectedStopIds] = useState<Set<string>>(
    new Set(),
  );
  const [selectedStopTypes, setSelectedStopTypes] = useState<Set<StopType>>(
    new Set(["Unassigned"]),
  );
  const [selectedMaxTrailerSizes, setSelectedMaxTrailerSizes] = useState<
    Set<TrailerSize>
  >(new Set(allMaxTrailerSizes));
  const [filter, setFilter] = useState("");

  const {
    sortedStops,
    allSortedStops,
    stopsByNeighborhood,
    ungroupedStops,
    teamExceedsSelection,
    sortedTeams,
    getTeamName,
  } = useDispatchDerivedState({
    stops,
    teams,
    neighborhoods,
    selectedStopIds,
    selectedStopTypes,
    selectedMaxTrailerSizes,
    filter,
  });

  const toggleStop = (stopId: string) => {
    setSelectedStopIds((prev) => {
      const newSet = new Set(prev);
      if (newSet.has(stopId)) {
        newSet.delete(stopId);
      } else {
        newSet.add(stopId);
      }
      return newSet;
    });
  };

  const clickTeam = (team: Team) => {
    if (teamExceedsSelection(team)) return;

    selectedStopIds.forEach((stopId) => {
      assignStopToTeam(campaignId!, stopId, team.id).then(updateStop);
    });
    setSelectedStopIds(new Set());
  };

  return (
    <NavigationPage>
      <div>
        <h1 className="text-xl font-bold">Dispatch</h1>
        <div className="flex gap-4 mt-4 items-start">
          <StopListPanel
            campaignId={campaignId}
            selectedStopTypes={selectedStopTypes}
            setSelectedStopTypes={setSelectedStopTypes}
            selectedMaxTrailerSizes={selectedMaxTrailerSizes}
            setSelectedMaxTrailerSizes={setSelectedMaxTrailerSizes}
            filter={filter}
            setFilter={setFilter}
            sortedStops={sortedStops}
            stopsByNeighborhood={stopsByNeighborhood}
            ungroupedStops={ungroupedStops}
            getTeamName={getTeamName}
            selectedStopIds={selectedStopIds}
            toggleStop={toggleStop}
            onSelectStops={(ids) => setSelectedStopIds(new Set(ids))}
          />
          <TeamListPanel
            campaignId={campaignId}
            sortedTeams={sortedTeams}
            stops={allSortedStops}
            assignMode={selectedStopIds.size > 0}
            teamExceedsSelection={teamExceedsSelection}
            onTeamClick={clickTeam}
            onUpdateStop={updateStop}
            onUpdateTeam={updateTeam}
          />
        </div>
      </div>
    </NavigationPage>
  );
}
