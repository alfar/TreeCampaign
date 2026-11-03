import { useRef, useState } from "react";
import type { Meta, StoryObj } from "@storybook/react-vite";
import { http, HttpResponse } from "msw";
import { expect, userEvent, waitFor, within } from "storybook/test";

import { AddressPicker, type Address, type AddressPickerHandle } from "./AddressPicker";

const MAX_HOUSE_NUMBER = 50;

// Several streets share the "Vester" prefix, so a short search gives multiple matches.
const streets = [
  { id: "aaaaaaaa-0000-0000-0000-000000000001", name: "Vestergade", zipCode: "8600" },
  { id: "aaaaaaaa-0000-0000-0000-000000000002", name: "Vestervang", zipCode: "8600" },
  { id: "aaaaaaaa-0000-0000-0000-000000000003", name: "Vesterbrogade", zipCode: "8600" },
  { id: "aaaaaaaa-0000-0000-0000-000000000004", name: "Vestre Ringvej", zipCode: "8600" },
  { id: "aaaaaaaa-0000-0000-0000-000000000005", name: "Østergade", zipCode: "8600" },
  { id: "aaaaaaaa-0000-0000-0000-000000000006", name: "Godthåbsvej", zipCode: "8600" },
];

/**
 * Mocks the adressevaelger.dk lookups and the street list used to resolve the
 * selected street. Try typing "Vest" or "Vester" to get several suggestions.
 */
function handlers() {
  return [
    http.get("/api/streets/:zipCode", () => HttpResponse.json(streets)),
    http.get("https://adressevaelger.dk/soeg", ({ request }) => {
      const query = (new URL(request.url).searchParams.get("vejnavn") ?? "").toLowerCase();
      const fund = streets
        .filter((s) => s.name.toLowerCase().includes(query))
        .map((s) => ({
          id: s.id,
          vejnavn: s.name,
          postnr: s.zipCode,
          postdistrikt: "Silkeborg",
          antal_husnumre: MAX_HOUSE_NUMBER,
        }));
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
  ];
}

function Harness(props: Partial<React.ComponentProps<typeof AddressPicker>>) {
  const [address, setAddress] = useState<Address | null>(null);
  return (
    <div className="max-w-xl space-y-4">
      <AddressPicker defaultZipCode="8600" onChange={setAddress} {...props} />
      <pre className="text-xs bg-gray-100 rounded p-2">{JSON.stringify(address, null, 2)}</pre>
    </div>
  );
}

const meta = {
  title: "Shared/AddressPicker",
  component: AddressPicker,
  render: (args) => <Harness {...args} />,
  args: { onChange: () => {} },
  parameters: { layout: "padded", msw: { handlers: handlers() } },
} satisfies Meta<typeof AddressPicker>;

export default meta;
type Story = StoryObj<typeof meta>;

/** Empty picker with zip code 8600. "Vester" matches four streets. */
export const MultipleMatches: Story = {
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    await userEvent.type(canvas.getByPlaceholderText("Søg gade..."), "Vester");

    await waitFor(() => expect(canvas.getAllByRole("listitem").length).toBeGreaterThan(1));
  },
};

/** Tab accepts the first (highlighted) suggestion and moves focus to the house number. */
export const TabSelectsFirstMatch: Story = {
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    await userEvent.type(canvas.getByPlaceholderText("Søg gade..."), "Vester");
    await waitFor(() => expect(canvas.getAllByRole("listitem").length).toBeGreaterThan(1));

    const first = canvas.getAllByRole("listitem")[0].textContent;
    await userEvent.tab();

    await waitFor(() => expect(canvas.getByPlaceholderText("Søg gade...")).toHaveValue(first));
    await expect(canvas.getByPlaceholderText("12A")).toHaveFocus();
  },
};

/** Arrow keys move the highlight (wrapping around); Tab then accepts the highlighted suggestion. */
export const ArrowKeysNavigate: Story = {
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    const street = canvas.getByPlaceholderText("Søg gade...");
    await userEvent.type(street, "Vester");
    await waitFor(() => expect(canvas.getAllByRole("listitem").length).toBeGreaterThan(1));

    const items = canvas.getAllByRole("listitem");
    await userEvent.keyboard("{ArrowDown}{ArrowDown}");
    await expect(items[2]).toHaveClass("bg-blue-50");

    await userEvent.keyboard("{ArrowUp}{ArrowUp}{ArrowUp}");
    await expect(items[items.length - 1]).toHaveClass("bg-blue-50");

    const name = items[items.length - 1].textContent;
    await userEvent.tab();
    await waitFor(() => expect(street).toHaveValue(name));
    await expect(canvas.getByPlaceholderText("12A")).toHaveFocus();
  },
};

/** Clicking a suggestion other than the first also moves focus to the house number. */
export const ClickSelectsStreet: Story = {
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    await userEvent.type(canvas.getByPlaceholderText("Søg gade..."), "Vester");
    await waitFor(() => expect(canvas.getAllByRole("listitem").length).toBeGreaterThan(1));

    const second = canvas.getAllByRole("listitem")[1];
    const name = second.textContent;
    await userEvent.click(second);

    await waitFor(() => expect(canvas.getByPlaceholderText("Søg gade...")).toHaveValue(name));
    await expect(canvas.getByPlaceholderText("12A")).toHaveFocus();
  },
};

/** The parent calls `reset()` through a ref to restore the defaults (here: Vestergade 12). */
export const Reset: Story = {
  render: () => {
    const pickerRef = useRef<AddressPickerHandle>(null);
    return (
      <div className="space-y-3">
        <Harness ref={pickerRef} defaultStreetName="Vestergade" defaultHouseNumber="12" />
        <button type="button" className="border rounded-lg px-3 py-2" onClick={() => pickerRef.current?.reset()}>
          Nulstil
        </button>
      </div>
    );
  },
  play: async ({ canvasElement }) => {
    const canvas = within(canvasElement);
    const street = canvas.getByPlaceholderText("Søg gade...");
    const houseNumber = canvas.getByPlaceholderText("12A");
    await waitFor(() => expect(canvas.getByText("Adresse fundet")).toBeInTheDocument());

    await userEvent.clear(street);
    await userEvent.type(street, "Vesterb");
    await userEvent.clear(houseNumber);
    await userEvent.type(houseNumber, "7");

    await userEvent.click(canvas.getByRole("button", { name: "Nulstil" }));

    await waitFor(() => expect(street).toHaveValue("Vestergade"));
    await expect(houseNumber).toHaveValue("12");
    await waitFor(() => expect(canvas.getByText("Adresse fundet")).toBeInTheDocument());
  },
};

/** Pre-filled street is selected silently, without stealing focus. */
export const Prefilled: Story = {
  render: () => <Harness defaultStreetName="Vestergade" defaultHouseNumber="12" />,
};
