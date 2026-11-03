import type { Meta, StoryObj } from "@storybook/react-vite";
import { http, HttpResponse, delay } from "msw";

import PublicIntakeForm from "./PublicIntakeForm";
import type { Street } from "../../shared/api/models/street";
import type {
  CampaignSummary,
  PendingOrderResult,
  ResolvedStreetSectionAddress,
} from "../../shared/api/client";

const campaignId = "11111111-1111-1111-1111-111111111111";
const territoryId = "22222222-2222-2222-2222-222222222222";

const streets: Street[] = [
  { id: "aaaaaaaa-0000-0000-0000-000000000001", name: "Vestergade", zipCode: "8600" },
  { id: "aaaaaaaa-0000-0000-0000-000000000002", name: "Østergade", zipCode: "8600" },
  { id: "aaaaaaaa-0000-0000-0000-000000000003", name: "Godthåbsvej", zipCode: "8600" },
];

const MAX_HOUSE_NUMBER = 50;

function randomPaymentCode() {
  const alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
  return Array.from({ length: 4 }, () => alphabet[Math.floor(Math.random() * alphabet.length)]).join("");
}

/**
 * Mocks the whole public flow. Try:
 *  - type a street name for suggestions; "Ørnevej" is suggested but outside the territory
 *  - house numbers 1–50 are valid; anything higher is "outside the area"
 *  - the order is created after a short delay and returns a payment code
 */
function handlers(options: { territoryId?: string | null; failOrder?: boolean } = {}) {
  const summary: CampaignSummary = {
    id: campaignId,
    territoryId: options.territoryId === undefined ? territoryId : options.territoryId,
  };

  return [
    http.get(`/api/campaigns/${campaignId}/summary`, () => HttpResponse.json(summary)),
    http.get(`/api/territories/${territoryId}/streets`, async () => {
      await delay(300);
      return HttpResponse.json(streets);
    }),
    http.get(`/api/territories/${territoryId}/street-sections/resolve`, async ({ request }) => {
      await delay(400);
      const url = new URL(request.url);
      const streetId = url.searchParams.get("streetId") ?? "";
      const houseNumber = url.searchParams.get("houseNumber") ?? "";
      const numeric = parseInt(houseNumber, 10);

      if (!Number.isFinite(numeric) || numeric < 1 || numeric > MAX_HOUSE_NUMBER) {
        return new HttpResponse(null, { status: 404 });
      }

      const resolved: ResolvedStreetSectionAddress = {
        territoryId,
        streetId,
        streetSectionId: "bbbbbbbb-0000-0000-0000-000000000001",
        neighborhoodId: "cccccccc-0000-0000-0000-000000000001",
        houseNumber,
      };
      return HttpResponse.json(resolved);
    }),
    // AddressPicker internals
    http.get("/api/streets/:zipCode", () => HttpResponse.json(streets)),
    http.get("https://adressevaelger.dk/soeg", ({ request }) => {
      const query = (new URL(request.url).searchParams.get("vejnavn") ?? "").toLowerCase();
      const fund = [...streets, { id: "outside", name: "Ørnevej", zipCode: "8600" }]
        .filter((s) => s.name.toLowerCase().includes(query))
        .map((s) => ({ id: s.id, vejnavn: s.name, postnr: "8600", postdistrikt: "Silkeborg", antal_husnumre: MAX_HOUSE_NUMBER }));
      return HttpResponse.json({ status: "ok", beskrivelse: "", fund });
    }),
    http.get("https://adressevaelger.dk/husnumre/soeg/", ({ request }) => {
      const vejnavn = new URL(request.url).searchParams.get("vejnavn") ?? "";
      const fund = Array.from({ length: MAX_HOUSE_NUMBER }, (_, i) => ({
        id: `${vejnavn}-${i + 1}`,
        vejnavn,
        husnummer: String(i + 1),
      }));
      return HttpResponse.json({ status: "ok", beskrivelse: "", fund });
    }),
    http.post(`/api/campaigns/${campaignId}/orders/pending`, async ({ request }) => {
      await delay(600);
      if (options.failOrder) {
        return new HttpResponse(null, { status: 500 });
      }
      const body = (await request.json()) as { amount: number };
      const result: PendingOrderResult = {
        id: crypto.randomUUID(),
        paymentCode: randomPaymentCode(),
        amount: body.amount,
        orderDate: new Date().toISOString(),
      };
      return HttpResponse.json(result);
    }),
  ];
}

const meta = {
  title: "Features/PublicIntake/PublicIntakeForm",
  component: PublicIntakeForm,
  args: { campaignId },
  parameters: { layout: "padded" },
} satisfies Meta<typeof PublicIntakeForm>;

export default meta;
type Story = StoryObj<typeof meta>;

/** Full happy path: pick street → validate address (1–50) → set trees → create order → see payment code. */
export const FullFlow: Story = {
  parameters: { msw: { handlers: handlers() } },
};

/** Order creation fails, to see the error state. */
export const OrderCreationFails: Story = {
  parameters: { msw: { handlers: handlers({ failOrder: true }) } },
};

/** Campaign has no territory configured. */
export const NoTerritory: Story = {
  parameters: { msw: { handlers: handlers({ territoryId: null }) } },
};
