import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getTerritories } from "../../shared/api/client";
import type { Territory } from "../../shared/api/models/territory";
import CreateTerritoryForm from "./CreateTerritoryForm";
import NavigationPage from "../../shared/components/NavigationPage";
import Button from "../../components/Button";

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
          <Button onClick={() => setShowForm((v) => !v)}>
            {showForm ? "Annuller" : "Nyt territorium"}
          </Button>
        </div>
        {showForm && <CreateTerritoryForm onCreated={handleCreated} />}
        {territories.map((t) => (
          <Link to={`/territories/${t.id}`} key={t.id} className="flex flex-col p-4 border rounded">
            <h2 className="text-lg font-semibold">
              {t.name}
            </h2>
            <p className="text-sm text-gray-500">{t.defaultZipCode}</p>
          </Link>
        ))}
      </div>
    </NavigationPage>
  );
}
