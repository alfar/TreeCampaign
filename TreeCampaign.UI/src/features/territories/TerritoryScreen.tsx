import { Fragment, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  getTerritory,
  getNeighborhoods,
  getStreetsByZipCode,
  deleteStreetSection,
} from "../../shared/api/client";
import type { Territory } from "../../shared/api/models/territory";
import type { Neighborhood } from "../../shared/api/models/neighborhood";
import type { Street } from "../../shared/api/models/street";
import type { StreetSection } from "../../shared/api/models/streetSection";
import NavigationPage from "../../shared/components/NavigationPage";
import Button from "../../components/Button";
import CreateNeighborhoodForm from "./CreateNeighborhoodForm";
import CreateStreetSectionForm from "./CreateStreetSectionForm";
import EditStreetSectionForm from "./EditStreetSectionForm";

function houseNumberRange(section: StreetSection): string {
  if (section.startHouseNumber == null && section.endHouseNumber == null)
    return "alle numre";
  if (section.startHouseNumber == null) return `– ${section.endHouseNumber}`;
  if (section.endHouseNumber == null) return `${section.startHouseNumber} –`;
  return `${section.startHouseNumber} – ${section.endHouseNumber}`;
}

export default function TerritoryScreen() {
  const { id } = useParams<{ id: string }>();
  const [territory, setTerritory] = useState<Territory | null>(null);
  const [neighborhoods, setNeighborhoods] = useState<Neighborhood[]>([]);
  const [streetIndex, setStreetIndex] = useState<Map<string, Street>>(
    new Map(),
  );
  const [showCreateNeighborhood, setShowCreateNeighborhood] = useState(false);
  const [activeStreetSectionNeighborhoodId, setActiveStreetSectionNeighborhoodId] = useState<string | null>(null);
  const [editingSectionId, setEditingSectionId] = useState<string | null>(null);
  const [deletingSectionId, setDeletingSectionId] = useState<string | null>(null);

  const handleDeleteSection = async (neighborhoodId: string, sectionId: string) => {
    if (!territory) return;
    setDeletingSectionId(sectionId);
    try {
      const res = await deleteStreetSection(territory.id, neighborhoodId, sectionId);
      if (res.ok) {
        const updatedHood: Neighborhood = await res.json();
        setNeighborhoods((prev) =>
          prev.map((h) => (h.id === updatedHood.id ? updatedHood : h)),
        );
      }
    } finally {
      setDeletingSectionId(null);
    }
  };

  const refreshStreetIndex = (zipCode: string) => {
    getStreetsByZipCode(zipCode).then((streets) => {
      setStreetIndex(new Map(streets.map((s) => [s.id, s])));
    });
  };

  useEffect(() => {
    if (!id) return;
    Promise.all([getTerritory(id), getNeighborhoods(id)])
      .then(([t, hoods]) => {
        setTerritory(t);
        setNeighborhoods(hoods);
        refreshStreetIndex(t.defaultZipCode);
      });
  }, [id]);

  if (!territory) return <div className="p-4">Indlæser…</div>;

  return (
    <NavigationPage>
      <div className="p-4 space-y-6">
        <div className="flex items-start justify-between">
          <div>
            <h1 className="text-xl font-bold">{territory.name}</h1>
            <p className="text-sm text-gray-500">
              Postnr. {territory.defaultZipCode}
            </p>
          </div>
          {!showCreateNeighborhood && (
            <Button onClick={() => setShowCreateNeighborhood(true)}>
              Nyt kvarter
            </Button>
          )}
        </div>

        {showCreateNeighborhood && (
          <CreateNeighborhoodForm
            territoryId={territory.id}
            onCreated={(hood) => {
              setNeighborhoods((prev) => [...prev, hood]);
              setShowCreateNeighborhood(false);
            }}
            onCancel={() => setShowCreateNeighborhood(false)}
          />
        )}

        {neighborhoods.length === 0 && (
          <p className="text-sm text-gray-500">Ingen kvarterer endnu.</p>
        )}

        {neighborhoods.map((hood) => (
          <div key={hood.id} className="border rounded p-4 space-y-3">
            <div className="flex items-center justify-between">
              <h2 className="font-semibold">{hood.name}</h2>
              {activeStreetSectionNeighborhoodId !== hood.id && (
                <Button
                  variant="secondary"
                  onClick={() => setActiveStreetSectionNeighborhoodId(hood.id)}
                >
                  + Vejstrækning
                </Button>
              )}
            </div>

            {activeStreetSectionNeighborhoodId === hood.id && (
              <CreateStreetSectionForm
                territoryId={territory.id}
                neighborhoodId={hood.id}
                defaultZipCode={territory.defaultZipCode}
                onCreated={(updatedHood) => {
                  setNeighborhoods((prev) =>
                    prev.map((h) => (h.id === updatedHood.id ? updatedHood : h)),
                  );
                  setActiveStreetSectionNeighborhoodId(null);
                  refreshStreetIndex(territory.defaultZipCode);
                }}
                onCancel={() => setActiveStreetSectionNeighborhoodId(null)}
              />
            )}

            {hood.streetSections.length === 0 ? (
              <p className="text-sm text-gray-400">Ingen gadeafsnit.</p>
            ) : (
              <table className="w-full text-sm">
                <thead>
                  <tr className="text-left text-gray-500 border-b">
                    <th className="pb-1 pr-4 font-medium">Gade</th>
                    <th className="pb-1 pr-4 font-medium">Numre</th>
                    <th className="pb-1 pr-4 font-medium">Rækkefølge</th>
                    <th className="pb-1 pr-4 font-medium">Retning</th>
                    <th className="pb-1 font-medium"></th>
                  </tr>
                </thead>
                <tbody>
                  {[...hood.streetSections]
                    .sort((a, b) => a.sortOrder - b.sortOrder)
                    .map((section) => (
                      <Fragment key={section.id}>
                        <tr className="border-b last:border-0">
                          <td className="py-1.5 pr-4">
                            {streetIndex.get(section.streetId)?.name ?? (
                              <span className="text-gray-400 italic">ukendt</span>
                            )}
                          </td>
                          <td className="py-1.5 pr-4 text-gray-600">
                            {houseNumberRange(section)}
                          </td>
                          <td className="py-1.5 pr-4 text-gray-600">
                            {section.sortOrder}
                          </td>
                          <td className="py-1.5 pr-4 text-gray-600">
                            {section.direction === 0 ? "Stigende" : "Faldende"}
                          </td>
                          <td className="py-1.5 text-right whitespace-nowrap">
                            {editingSectionId !== section.id && (
                              <Button
                                variant="secondary"
                                className="mr-2"
                                onClick={() => setEditingSectionId(section.id)}
                              >
                                Rediger
                              </Button>
                            )}
                            <Button
                              variant="danger"
                              onClick={() => handleDeleteSection(hood.id, section.id)}
                              disabled={deletingSectionId === section.id}
                            >
                              {deletingSectionId === section.id ? "Sletter…" : "Slet"}
                            </Button>
                          </td>
                        </tr>
                        {editingSectionId === section.id && (
                          <tr className="border-b last:border-0">
                            <td colSpan={5} className="py-2">
                              <EditStreetSectionForm
                                territoryId={territory.id}
                                neighborhoodId={hood.id}
                                section={section}
                                onSaved={(updatedHood) => {
                                  setNeighborhoods((prev) =>
                                    prev.map((h) => (h.id === updatedHood.id ? updatedHood : h)),
                                  );
                                  setEditingSectionId(null);
                                }}
                                onCancel={() => setEditingSectionId(null)}
                              />
                            </td>
                          </tr>
                        )}
                      </Fragment>
                    ))}
                </tbody>
              </table>
            )}
          </div>
        ))}
      </div>
    </NavigationPage>
  );
}
