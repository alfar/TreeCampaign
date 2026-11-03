import { useState, useEffect, useRef, useImperativeHandle, type Ref } from "react";
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

export interface AddressPickerHandle {
  /** Restores zip code, street and house number to the default* props and focuses the street field. */
  reset: () => void;
}

export function AddressPicker({
  defaultZipCode = "",
  defaultStreetName = "",
  defaultHouseNumber = "",
  onChange,
  ref,
}: {
  defaultZipCode?: string;
  defaultStreetName?: string;
  defaultHouseNumber?: string;
  onChange: (address: Address) => void;
  ref?: Ref<AddressPickerHandle>;
}) {
  const [zipCode, setZipCode] = useState(defaultZipCode);
  const [streetSearch, setStreetSearch] = useState(defaultStreetName);
  const [selectedStreetName, setSelectedStreetName] = useState<string | null>(null);
  const [streetCandidates, setStreetCandidates] = useState<StreetCandidate[]>([]);
  const [houseNumber, setHouseNumber] = useState(defaultHouseNumber);
  const [houseNumberCandidates, setHouseNumberCandidates] = useState<HouseNumberCandidate[]>([]);
  const [resolvedStreet, setResolvedStreet] = useState<Street | null>(null);

  const houseNumberRef = useRef<HTMLInputElement>(null);
  const streetRef = useRef<HTMLInputElement>(null);
  const [activeStreetIndex, setActiveStreetIndex] = useState(0);

  useImperativeHandle(ref, () => ({
    reset() {
      setZipCode(defaultZipCode);
      setStreetSearch(defaultStreetName);
      setSelectedStreetName(null);
      setStreetCandidates([]);
      setHouseNumber(defaultHouseNumber);
      streetRef.current?.focus();
    },
  }));

  useEffect(() => {
    setActiveStreetIndex(0);
  }, [streetCandidates]);

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

  function selectStreet(candidate: StreetCandidate) {
    setSelectedStreetName(candidate.vejnavn);
    setStreetCandidates([]);
    setHouseNumber("");
    houseNumberRef.current?.focus();
  }

  return (
    <div className="flex flex-wrap gap-3">
      <div className="w-full sm:flex-1 relative">
        <label className="text-sm font-medium text-gray-700 block mb-1">Gadenavn</label>
        <input
          ref={streetRef}
          type="text"
          placeholder="Søg gade..."
          value={streetName}
          onChange={(e) => {
            setStreetSearch(e.target.value);
            setSelectedStreetName(null);
            setHouseNumber("");
          }}
          onKeyDown={(e) => {
            if (selectedStreetName || streetCandidates.length === 0) return;

            if (e.key === "ArrowDown" || e.key === "ArrowUp") {
              e.preventDefault();
              const step = e.key === "ArrowDown" ? 1 : -1;
              setActiveStreetIndex((i) => (i + step + streetCandidates.length) % streetCandidates.length);
            } else if ((e.key === "Tab" && !e.shiftKey) || e.key === "Enter") {
              e.preventDefault();
              selectStreet(streetCandidates[activeStreetIndex] ?? streetCandidates[0]);
            }
          }}
          className="w-full border rounded-lg px-3 py-2" />
        {streetCandidates.length > 0 && !selectedStreetName && (
          <ul className="absolute z-10 w-full bg-white border rounded-lg mt-1 max-h-48 overflow-y-auto shadow">
            {streetCandidates.map((c, i) => (
              <li
                key={c.id}
                ref={i === activeStreetIndex ? (el) => el?.scrollIntoView?.({ block: "nearest" }) : undefined}
                className={`px-3 py-2 cursor-pointer hover:bg-blue-50 ${i === activeStreetIndex ? "bg-blue-50" : ""}`}
                onClick={() => selectStreet(c)}
              >
                {c.vejnavn}
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className="flex-1 sm:flex-none sm:w-28 relative">
        <label className="text-sm font-medium text-gray-700 block mb-1">Husnummer</label>
        <input
          ref={houseNumberRef}
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

      <div className="flex-1 sm:flex-none sm:w-24">
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
