import type { PropsWithChildren } from "react";
import { Link, useParams } from "react-router-dom";

export default function NavigationPage({ children } : PropsWithChildren) {
  const params = useParams();
  const campaignId = params["campaignId"];

  return (
    <div className="flex bg-blue-800 min-h-svh">
      <div className="w-2/12">
        <nav className="flex flex-col gap-2 p-4">
          <Link className="text-white hover:text-blue-200" to="/">Kampagner</Link>
          <Link className="text-white hover:text-blue-200" to="/territories">Territorier</Link>
          {campaignId && <Link className="text-white hover:text-blue-200" to={`/campaigns/${campaignId}/intake`}>Ordrer</Link>}
          {campaignId && <Link className="text-white hover:text-blue-200" to={`/campaigns/${campaignId}/dispatch`}>Dispatch</Link>}
          {campaignId && <Link className="text-white hover:text-blue-200" to={`/campaigns/${campaignId}/overview-map`}>Kort</Link>}
        </nav>
      </div>
      <div className="w-10/12 bg-white rounded-tl-2xl p-4">{children}</div>
    </div>
  );
}
