import { useEffect, useState } from "react";
import { getCampaigns } from "../../shared/api/client";
import { Link } from "react-router-dom";
import type { Campaign } from "../../shared/api/models/campagin";
import CreateCampaignForm from "./CreateCampaignForm";
import UpdateCampaignForm from "./UpdateCampaignForm";

export default function CampaignListScreen() {
    const [campaigns, setCampaigns] = useState<Campaign[]>([]);
    const [showCreateForm, setShowCreateForm] = useState(false);
    const [editingId, setEditingId] = useState<string | null>(null);

    useEffect(() => {
        getCampaigns().then(setCampaigns);
    }, []);

    const handleCreated = (campaign: Campaign) => {
        setCampaigns((prev) => [...prev, campaign]);
        setShowCreateForm(false);
    };

    const handleUpdated = (campaign: Campaign) => {
        setCampaigns((prev) => prev.map((c) => (c.id === campaign.id ? campaign : c)));
        setEditingId(null);
    };

    return (
        <div className="p-4 space-y-4">
            <div className="flex items-center justify-between">
                <h1 className="text-xl font-bold">Kampagner</h1>
                <button
                    onClick={() => { setShowCreateForm((v) => !v); setEditingId(null); }}
                    className="text-sm bg-blue-600 text-white py-1.5 px-4 rounded"
                >
                    {showCreateForm ? "Annuller" : "Ny kampagne"}
                </button>
            </div>
            {showCreateForm && <CreateCampaignForm onCreated={handleCreated} />}
            {campaigns.map(c => (
                <div key={c.id} className="p-4 border rounded">
                    <div className="flex items-center justify-between">
                        <h2 className="text-lg font-semibold">
                            <Link to={`/campaigns/${c.id}/dispatch`}>{c.season}</Link>
                        </h2>
                        <button
                            onClick={() => { setEditingId((prev) => prev === c.id ? null : c.id); setShowCreateForm(false); }}
                            className="text-gray-400 hover:text-gray-700"
                            aria-label="Rediger"
                        >
                            <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4" viewBox="0 0 20 20" fill="currentColor">
                                <path d="M13.586 3.586a2 2 0 112.828 2.828l-.793.793-2.828-2.828.793-.793zM11.379 5.793L3 14.172V17h2.828l8.38-8.379-2.83-2.828z" />
                            </svg>
                        </button>
                    </div>
                    {editingId === c.id && (
                        <UpdateCampaignForm
                            campaign={c}
                            onUpdated={handleUpdated}
                            onCancel={() => setEditingId(null)}
                        />
                    )}
                </div>
            ))}
        </div>
    );
}