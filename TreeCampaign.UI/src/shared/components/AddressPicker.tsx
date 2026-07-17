import { useState, useEffect } from "react";
import { getStreetsByZipCode } from "../api/client";
import { searchStreets, searchHouseNumbers, type StreetCandidate, type HouseNumberCandidate } from "../api/adressevaelger";
import type { Street } from "../api/models/street";

export interface Address {
  zipCode: string;
  street: Street | null;
  streetName: string;
  houseNumber: string;
  isValid: boolean | null;
}

export function AddressPicker({
  defaultZipCode = "",
  defaultStreetName = "",
  defaultHouseNumber = "",
  onChange,
}: {
  defaultZipCode?: string;
  defaultStreetName?: string;
  defaultHouseNumber?: string;
  onChange: (address: Address) => void;
}) {
  const [zipCode, setZipCode] = useState(defaultZipCode);
  const [streetSearch, setStreetSearch] = useState(defaultStreetName);
  const [selectedStreetName, setSelectedStreetName] = useState<string | null>(null);
  const [streetCandidates, setStreetCandidates] = useState<StreetCandidate[]>([]);
  const [houseNumber, setHouseNumber] = useState(defaultHouseNumber);
  const [houseNumberCandidates, setHouseNumberCandidates] = useState<HouseNumberCandidate[]>([]);
  const [resolvedStreet, setResolvedStreet] = useState<Street | null>(null);

  const streetName = selectedStreetName ?? streetSearch;

  useEffect(() => {
    setZipCode(defaultZipCode);
  }, [defaultZipCode]);

  useEffect(() => {
    setStreetSearch(defaultStreetName);
    setSelectedStreetName(null);
  }, [defaultStreetName]);

  useEffect(() => {
    setHouseNumber(defaultHouseNumber);
  }, [defaultHouseNumber]);

  useEffect(() => {
    if (selectedStreetName || zipCode.length !== 4 || streetSearch.trim().length < 2) {
      setStreetCandidates([]);
      return;
    }

    let cancelled = false;
    const timeout = setTimeout(() => {
      searchStreets(streetSearch.trim(), zipCode).then((candidates) => {
        if (!cancelled) setStreetCandidates(candidates);
      });
    }, 300);

    return () => {
      cancelled = true;
      clearTimeout(timeout);
    };
  }, [streetSearch, zipCode, selectedStreetName]);

  useEffect(() => {
    if (
      selectedStreetName ||
      !defaultStreetName.trim() ||
      streetSearch !== defaultStreetName ||
      zipCode.length !== 4
    ) {
      return;
    }

    let cancelled = false;
    searchStreets(defaultStreetName.trim(), zipCode).then((candidates) => {
      if (cancelled) return;
      const exactMatch = candidates.find(
        (c) => c.vejnavn.toLowerCase() === defaultStreetName.trim().toLowerCase(),
      );
      if (exactMatch) {
        setSelectedStreetName(exactMatch.vejnavn);
        setStreetCandidates([]);
      }
    });

    return () => {
      cancelled = true;
    };
  }, [defaultStreetName, streetSearch, zipCode, selectedStreetName]);

  useEffect(() => {
    if (!selectedStreetName || zipCode.length !== 4) {
      setHouseNumberCandidates([]);
      return;
    }

    let cancelled = false;
    searchHouseNumbers(selectedStreetName, zipCode).then((candidates) => {
      if (!cancelled) {
        setHouseNumberCandidates(candidates.filter((c) => typeof c.husnummer === "string"));
      }
    });

    return () => {
      cancelled = true;
    };
  }, [selectedStreetName, zipCode]);

  useEffect(() => {
    if (!selectedStreetName || zipCode.length !== 4) {
      setResolvedStreet(null);
      return;
    }

    let cancelled = false;
    getStreetsByZipCode(zipCode).then((streets) => {
      if (cancelled) return;
      const match = streets.find((s) => s.name.toLowerCase() === selectedStreetName.toLowerCase());
      setResolvedStreet(match ?? null);
    });

    return () => {
      cancelled = true;
    };
  }, [selectedStreetName, zipCode]);

  const isValid = houseNumber.trim().length === 0
    ? null
    : houseNumberCandidates.some((c) => c.husnummer?.toLowerCase() === houseNumber.trim().toLowerCase());

  useEffect(() => {
    onChange({ zipCode, street: resolvedStreet, streetName, houseNumber, isValid });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [zipCode, resolvedStreet, streetName, houseNumber, isValid]);

  const filteredHouseNumbers = houseNumberCandidates.filter((c) => c.husnummer?.toLowerCase().startsWith(houseNumber.trim().toLowerCase())
  );

  return (
    <div className="flex gap-3">
      <div className="flex-1 relative">
        <label className="text-sm font-medium text-gray-700 block mb-1">Gadenavn</label>
        <input
          type="text"
          placeholder="Søg gade..."
          value={streetName}
          onChange={(e) => {
            setStreetSearch(e.target.value);
            setSelectedStreetName(null);
            setHouseNumber("");
          }}
          className="w-full border rounded-lg px-3 py-2" />
        {streetCandidates.length > 0 && !selectedStreetName && (
          <ul className="absolute z-10 w-full bg-white border rounded-lg mt-1 max-h-48 overflow-y-auto shadow">
            {streetCandidates.map((c) => (
              <li
                key={c.id}
                className="px-3 py-2 cursor-pointer hover:bg-blue-50"
                onClick={() => {
                  setSelectedStreetName(c.vejnavn);
                  setStreetCandidates([]);
                  setHouseNumber("");
                }}
              >
                {c.vejnavn}
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className="w-28 relative">
        <label className="text-sm font-medium text-gray-700 block mb-1">Husnummer</label>
        <input
          type="text"
          placeholder="12A"
          value={houseNumber}
          onChange={(e) => setHouseNumber(e.target.value)}
          className={`w-full border rounded-lg px-3 py-2 ${isValid === false ? "border-red-500" : ""}`} />
        {houseNumber.trim() && isValid === false && filteredHouseNumbers.length > 0 && (
          <ul className="absolute z-10 w-full bg-white border rounded-lg mt-1 max-h-48 overflow-y-auto shadow">
            {filteredHouseNumbers.map((c) => (
              <li
                key={c.id}
                className="px-3 py-2 cursor-pointer hover:bg-blue-50"
                onClick={() => setHouseNumber(c.husnummer)}
              >
                {c.husnummer}
              </li>
            ))}
          </ul>
        )}
        {isValid === false && <p className="text-xs text-red-600 mt-1">Findes ikke</p>}
        {isValid === true && <p className="text-xs text-green-600 mt-1">Adresse fundet</p>}
      </div>

      <div className="w-24">
        <label className="text-sm font-medium text-gray-700 block mb-1">Postnr.</label>
        <input
          type="text"
          inputMode="numeric"
          maxLength={4}
          value={zipCode}
          onChange={(e) => {
            setZipCode(e.target.value);
            setSelectedStreetName(null);
            setStreetSearch("");
            setHouseNumber("");
          }}
          className="w-full border rounded-lg px-3 py-2" />
      </div>
    </div>
  );
}
