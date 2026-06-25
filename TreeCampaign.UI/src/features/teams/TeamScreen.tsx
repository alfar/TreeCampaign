import { NavLink, Outlet, useParams } from "react-router-dom";

export default function TeamScreen() {
  const { campaignId, teamId } = useParams();
  const base = `/campaigns/${campaignId}/teams/${teamId}`;

  const navLink = ({ isActive }: { isActive: boolean }) =>
    `flex-1 text-center py-2 text-sm font-medium ${
      isActive
        ? "border-b-2 border-blue-600 text-blue-600"
        : "text-gray-500 border-b-2 border-transparent"
    }`;

  return (
    <div>
      <div className="fixed top-0 w-full bg-white z-10">
        <nav className="flex">
          <NavLink to={`${base}/stops`} className={navLink}>
            Stop
          </NavLink>
          <NavLink to={`${base}/map`} className={navLink}>
            Kort
          </NavLink>
          <NavLink to={`${base}/info`} className={navLink}>
            Info
          </NavLink>
        </nav>
      </div>
      <div className="mt-16">
        <Outlet />
      </div>
    </div>
  );
}
