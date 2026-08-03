import { useEffect, useState } from "react";
import { getCampaigns } from "../../shared/api/client";
import { Link } from "react-router-dom";
import type { Campaign } from "../../shared/api/models/campagin";
import CreateCampaignForm from "./CreateCampaignForm";
import UpdateCampaignForm from "./UpdateCampaignForm";
import NavigationPage from "../../shared/components/NavigationPage";
import Button from "../../components/Button";
import { PencilIcon } from "@heroicons/react/24/outline";

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
    setCampaigns((prev) =>
      prev.map((c) => (c.id === campaign.id ? campaign : c)),
    );
    setEditingId(null);
  };

  return (
    <NavigationPage>
      <div className="p-4 space-y-4">
        <div className="flex items-center justify-between">
          <h1 className="text-xl font-bold">Kampagner</h1>
          <Button
            onClick={() => {
              setShowCreateForm((v) => !v);
              setEditingId(null);
            }}
          >
            {showCreateForm ? "Annuller" : "Ny kampagne"}
          </Button>
        </div>
        {showCreateForm && <CreateCampaignForm onCreated={handleCreated} />}
        {campaigns.map((c) => (
          <div key={c.id} className="p-4 border rounded">
            <div className="flex items-center justify-between">
              <Link to={`/campaigns/${c.id}/dispatch`} className="flex-1">
                <h2 className="text-lg font-semibold">
                  {c.season}
                </h2>
              </Link>
              <Button
                variant="secondary"
                onClick={() => {
                  setEditingId((prev) => (prev === c.id ? null : c.id));
                  setShowCreateForm(false);
                }}
              >
                <PencilIcon className="h-4 w-4" />
                Rediger
              </Button>
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
    </NavigationPage>
  );
}
