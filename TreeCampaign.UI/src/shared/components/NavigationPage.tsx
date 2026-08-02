import { useState, type PropsWithChildren } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { useAuth } from "../../app/AuthContext";

export default function NavigationPage({ children }: PropsWithChildren) {
  const params = useParams();
  const campaignId = params["campaignId"];
  const [collapsed, setCollapsed] = useState(false);
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate("/login", { replace: true });
  };

  return (
    <div className="flex bg-blue-800 min-h-svh">
      <div className="flex flex-col">
        {collapsed ? (
          <button
            type="button"
            onClick={() => setCollapsed(false)}
            title="Vis menu"
            className="flex-1 flex flex-col items-center pt-4 px-2 rounded hover:bg-blue-700 text-white"
          >
            <img src="/logo.png" alt="TreeCampaign logo" className="w-6 transition-all" />
          </button>
        ) : (
          <button
            type="button"
            onClick={() => setCollapsed(true)}
            title="Skjul menu"
            className="p-1 mt-4 rounded flex flex-col items-center text-white"
          >
            <img src="/logo.png" alt="TreeCampaign logo" className="w-12 transition-all" />
          </button>
        )}
        {!collapsed && (
          <nav className="flex flex-col gap-2 p-4 pr-8 w-fit">
            <Link
              className="text-white hover:text-blue-200 whitespace-nowrap"
              to="/"
            >
              Kampagner
            </Link>
            <Link
              className="text-white hover:text-blue-200 whitespace-nowrap"
              to="/territories"
            >
              Territorier
            </Link>
            {user && (
              <Link
                className="text-white hover:text-blue-200 whitespace-nowrap"
                to="/users"
              >
                Brugere
              </Link>
            )}
            {campaignId && (
              <Link
                className="text-white hover:text-blue-200 whitespace-nowrap"
                to={`/campaigns/${campaignId}/intake`}
              >
                Ordrer
              </Link>
            )}
            {campaignId && (
              <Link
                className="text-white hover:text-blue-200 whitespace-nowrap"
                to={`/campaigns/${campaignId}/dispatch`}
              >
                Dispatch
              </Link>
            )}
            {campaignId && (
              <Link
                className="text-white hover:text-blue-200 whitespace-nowrap"
                to={`/campaigns/${campaignId}/overview-map`}
              >
                Kort
              </Link>
            )}
          </nav>
        )}
        {!collapsed && user && (
          <div className="mt-auto flex flex-col gap-2 p-4 pr-8">
            <span className="text-blue-200 text-xs whitespace-nowrap">{user.displayName}</span>
            <button
              type="button"
              onClick={handleLogout}
              className="text-white hover:text-blue-200 text-sm text-left whitespace-nowrap"
            >
              Log ud
            </button>
          </div>
        )}
      </div>
      <div className="flex-1 bg-white rounded-tl-2xl p-4">{children}</div>
    </div>
  );
}
