import { useEffect, useState } from "react";
import { getCampaigns } from "../../shared/api/client";
import { Link } from "react-router-dom";
import type { Campaign } from "../../shared/api/models/campagin";

export default function CampaignListScreen() {
    const [campaigns, setCampaigns] = useState<Campaign[]>([]);

    useEffect(() => {
        getCampaigns().then(setCampaigns);
    }, []);

    return (
        <div className="p-4 space-y-4">
            <h1 className="text-xl font-bold">My Campaigns</h1>
            {campaigns.map(c => (
                <div key={c.id} className="p-4 border rounded">
                    <h2 className="text-lg font-semibold"><Link to={`/campaigns/${c.id}/dispatch`}>{c.season}</Link></h2>
                </div>
            ))}
        </div>
    );
}