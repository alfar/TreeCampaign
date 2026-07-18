import { useState, type PropsWithChildren } from "react";
import { Link, useParams } from "react-router-dom";

export default function NavigationPage({ children }: PropsWithChildren) {
  const params = useParams();
  const campaignId = params["campaignId"];
  const [collapsed, setCollapsed] = useState(false);

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
      </div>
      <div className="flex-1 bg-white rounded-tl-2xl p-4">{children}</div>
    </div>
  );
}
