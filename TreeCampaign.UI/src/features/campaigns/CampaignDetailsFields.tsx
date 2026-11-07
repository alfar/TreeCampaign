import type { CampaignDetailsValues } from "./campaignDetails";

interface CampaignDetailsFieldsProps {
  values: CampaignDetailsValues;
  onChange: (values: CampaignDetailsValues) => void;
}

export default function CampaignDetailsFields({ values, onChange }: CampaignDetailsFieldsProps) {
  const set = (patch: Partial<CampaignDetailsValues>) => onChange({ ...values, ...patch });

  return (
    <>
      <div>
        <label className="text-sm font-medium text-gray-700 block mb-1">Titel (intern)</label>
        <input
          type="text"
          value={values.title}
          onChange={(e) => set({ title: e.target.value })}
          className="w-full border rounded px-3 py-2 text-sm"
          maxLength={200}
        />
      </div>
      <div className="flex gap-3 flex-wrap">
        <div className="w-40">
          <label className="text-sm font-medium text-gray-700 block mb-1">Indsamlingsdato</label>
          <input
            type="date"
            value={values.collectionDate}
            onChange={(e) => set({ collectionDate: e.target.value })}
            className="w-full border rounded px-3 py-2 text-sm"
          />
        </div>
        <div className="w-32">
          <label className="text-sm font-medium text-gray-700 block mb-1">Pris pr. træ (kr.)</label>
          <input
            type="number"
            value={values.pricePerTree}
            onChange={(e) => set({ pricePerTree: e.target.value })}
            className="w-full border rounded px-3 py-2 text-sm"
            min={0}
            step="0.01"
          />
        </div>
        <div className="w-56">
          <label className="text-sm font-medium text-gray-700 block mb-1">Sidste bestillingstidspunkt</label>
          <input
            type="datetime-local"
            value={values.lastOrderDateTime}
            onChange={(e) => set({ lastOrderDateTime: e.target.value })}
            className="w-full border rounded px-3 py-2 text-sm"
          />
        </div>
      </div>
    </>
  );
}
