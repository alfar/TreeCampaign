import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  getTerritory,
  getNeighborhoods,
  getStreetsByZipCode,
} from "../../shared/api/client";
import type { Territory } from "../../shared/api/models/territory";
import type { Neighborhood } from "../../shared/api/models/neighborhood";
import type { Street } from "../../shared/api/models/street";
import type { StreetSection } from "../../shared/api/models/streetSection";
import NavigationPage from "../../shared/components/NavigationPage";

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

  useEffect(() => {
    if (!id) return;
    Promise.all([getTerritory(id), getNeighborhoods(id)])
      .then(([t, hoods]) => {
        setTerritory(t);
        setNeighborhoods(hoods);
        return getStreetsByZipCode(t.defaultZipCode);
      })
      .then((streets) => {
        setStreetIndex(new Map(streets.map((s) => [s.id, s])));
      });
  }, [id]);

  if (!territory) return <div className="p-4">Indlæser…</div>;

  return (
    <NavigationPage>
      <div className="p-4 space-y-6">
        <div>
          <h1 className="text-xl font-bold">{territory.name}</h1>
          <p className="text-sm text-gray-500">
            Postnr. {territory.defaultZipCode}
          </p>
        </div>

        {neighborhoods.length === 0 && (
          <p className="text-sm text-gray-500">Ingen kvarterer endnu.</p>
        )}

        {neighborhoods.map((hood) => (
          <div key={hood.id} className="border rounded p-4 space-y-3">
            <h2 className="font-semibold">{hood.name}</h2>
            {hood.streetSections.length === 0 ? (
              <p className="text-sm text-gray-400">Ingen gadeafsnit.</p>
            ) : (
              <table className="w-full text-sm">
                <thead>
                  <tr className="text-left text-gray-500 border-b">
                    <th className="pb-1 pr-4 font-medium">Gade</th>
                    <th className="pb-1 pr-4 font-medium">Numre</th>
                    <th className="pb-1 pr-4 font-medium">Rækkefølge</th>
                    <th className="pb-1 font-medium">Retning</th>
                  </tr>
                </thead>
                <tbody>
                  {[...hood.streetSections]
                    .sort((a, b) => a.sortOrder - b.sortOrder)
                    .map((section) => (
                      <tr key={section.id} className="border-b last:border-0">
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
                        <td className="py-1.5 text-gray-600">
                          {section.direction === 0 ? "Stigende" : "Faldende"}
                        </td>
                      </tr>
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
