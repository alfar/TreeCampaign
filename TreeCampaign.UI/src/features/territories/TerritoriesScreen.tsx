import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getTerritories } from "../../shared/api/client";
import type { Territory } from "../../shared/api/models/territory";
import CreateTerritoryForm from "./CreateTerritoryForm";
import NavigationPage from "../../shared/components/NavigationPage";

export default function TerritoriesScreen() {
  const [territories, setTerritories] = useState<Territory[]>([]);
  const [showForm, setShowForm] = useState(false);

  useEffect(() => {
    getTerritories().then(setTerritories);
  }, []);

  const handleCreated = (territory: Territory) => {
    setTerritories((prev) => [...prev, territory]);
    setShowForm(false);
  };

  return (
    <NavigationPage>
      <div className="p-4 space-y-4">
        <div className="flex items-center justify-between">
          <h1 className="text-xl font-bold">Territorier</h1>
          <button
            onClick={() => setShowForm((v) => !v)}
            className="text-sm bg-blue-600 text-white py-1.5 px-4 rounded"
          >
            {showForm ? "Annuller" : "Nyt territorium"}
          </button>
        </div>
        {showForm && <CreateTerritoryForm onCreated={handleCreated} />}
        {territories.map((t) => (
          <div key={t.id} className="p-4 border rounded">
            <h2 className="text-lg font-semibold">
              <Link to={`/territories/${t.id}`}>{t.name}</Link>
            </h2>
            <p className="text-sm text-gray-500">{t.defaultZipCode}</p>
          </div>
        ))}
      </div>
    </NavigationPage>
  );
}
