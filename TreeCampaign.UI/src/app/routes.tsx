import { createBrowserRouter, Navigate } from "react-router-dom";
import TeamScreen from "../features/teams/TeamScreen";
import TeamStopsTab from "../features/teams/TeamStopsTab";
import TeamMapTab from "../features/teams/TeamMapTab";
import TeamInfoTab from "../features/teams/TeamInfoTab";
import DispatchScreen from "../features/dispatch/DispatchScreen";
import OverviewMapScreen from "../features/overview-map/OverviewMapScreen";
import CampaignListScreen from "../features/campaigns/CampaignListScreen";
import IntakeScreen from "../features/intake/IntakeScreen";
import TerritoriesScreen from "../features/territories/TerritoriesScreen";
import TerritoryScreen from "../features/territories/TerritoryScreen";
import UserManagementScreen from "../features/users/UserManagementScreen";
import LoginScreen from "../features/auth/LoginScreen";
import RegisterScreen from "../features/auth/RegisterScreen";
import RequireAuth from "./RequireAuth";

export const router = createBrowserRouter([
  {
    path: "/login",
    element: <LoginScreen />
  },
  {
    path: "/register",
    element: <RegisterScreen />
  },
  {
    path: "/campaigns/:campaignId/teams/:teamId",
    element: <TeamScreen />,
    children: [
      { index: true, element: <Navigate to="stops" replace /> },
      { path: "stops", element: <TeamStopsTab /> },
      { path: "map", element: <TeamMapTab /> },
      { path: "info", element: <TeamInfoTab /> },
    ],
  },
  {
    element: <RequireAuth />,
    children: [
      {
        path: "/",
        element: <CampaignListScreen />
      },
      {
        path: "/campaigns/:campaignId/dispatch",
        element: <DispatchScreen />
      },
      {
        path: "/campaigns/:campaignId/overview-map",
        element: <OverviewMapScreen />
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
      },
      {
        path: "/users",
        element: <UserManagementScreen />
      }
    ]
  }
]);
