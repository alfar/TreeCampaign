import { createBrowserRouter } from "react-router-dom";
import TeamScreen from "../features/teams/TeamScreen";
import DispatchScreen from "../features/dispatch/DispatchScreen";
import CampaignListScreen from "../features/campaigns/CampaignListScreen";

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
  }
]);