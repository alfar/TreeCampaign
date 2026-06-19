import { createBrowserRouter } from "react-router-dom";
import TeamScreen from "../features/teams/TeamScreen";
import DispatchScreen from "../features/dispatch/DispatchScreen";
import CampaignListScreen from "../features/campaigns/CampaignListScreen";
import IntakeScreen from "../features/intake/IntakeScreen";
import TerritoriesScreen from "../features/territories/TerritoriesScreen";
import TerritoryScreen from "../features/territories/TerritoryScreen";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <CampaignListScreen />
  },
  {
    path: "/campaigns/:campaignId/teams/:teamId",
    element: <TeamScreen />
  },
  {
    path: "/campaigns/:campaignId/dispatch",
    element: <DispatchScreen />
  },
  {
    path: "/campaigns/:campaignId/intake",
    element: <IntakeScreen />
  },
  {
    path: "/territories",
    element: <TerritoriesScreen />
  },
  {
    path: "/territories/:id",
    element: <TerritoryScreen />
  }
]);