const HUSNUMRE_SOEG_URL = "https://adressevaelger.dk/husnumre/soeg/";
const SOEG_URL = "https://adressevaelger.dk/soeg";
const TOKEN = "adressevaelger123";

interface AdressevaelgerResponse<T> {
  status: string;
  beskrivelse: string;
  fund: T[];
}

export interface StreetCandidate {
  id: string;
  vejnavn: string;
  postnr: string;
  postdistrikt: string;
  antal_husnumre: number;
}

export interface HouseNumberCandidate {
  id: string;
  vejnavn: string;
  husnummer: string;
}

export async function searchStreets(streetName: string, zipCode: string): Promise<StreetCandidate[]> {
  const params = new URLSearchParams({
    vejnavn: streetName,
    postnummer: zipCode,
    token: TOKEN,
    maal: "navngivenvejpostnummer",
  });
  const res = await fetch(`${SOEG_URL}?${params}`);
  const data: AdressevaelgerResponse<StreetCandidate> = await res.json();
  return data.fund;
}

export async function searchHouseNumbers(streetName: string, zipCode: string, houseNumber?: string): Promise<HouseNumberCandidate[]> {
  const params = new URLSearchParams({
    vejnavn: streetName,
    postnummer: zipCode,
    token: TOKEN,
  });
  if (houseNumber) {
    params.set("husnummer", houseNumber);
  }
  const res = await fetch(`${HUSNUMRE_SOEG_URL}?${params}`);
  const data: AdressevaelgerResponse<HouseNumberCandidate> = await res.json();
  return data.fund;
}
