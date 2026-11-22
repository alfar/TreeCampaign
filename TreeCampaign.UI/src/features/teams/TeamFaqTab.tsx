import { useOutletContext } from "react-router-dom";
import ProgressPie from "../../components/ProgressPie";
import { apiUrl } from "../../shared/api/client";
import type { TeamScreenContext } from "./TeamScreen";

const MOBILEPAY_URL = "https://qr.mobilepay.dk/vp/OJbdGYMcM";
const DEFAULT_PRICE_PER_TREE = 40;

export default function TeamFaqTab() {
  const { progress, campaign } = useOutletContext<TeamScreenContext>();
  const pricePerTree = campaign?.pricePerTree ?? DEFAULT_PRICE_PER_TREE;

  const faq = [
    {
      question: "Hvordan får jeg hjælp med appen eller min rute?",
      answer:
        <>Ring til dispatch på <a href="tel:+4553674848" className="text-blue-500 font-bold text-lg">53 67 48 48</a></>,
    },
    {
      question: "Hvad gør jeg, hvis jeg modtager kontanter?",
      answer:
        "Tæl pengene op, overfør det samlede beløb via MobilePay (brug QR-koden her på siden), og tag kontanterne med hjem.",
    },
    {
      question: "Hvordan kan en kunde betale på dagen?",
      answer:
        "Tag imod kontanter eller lad dem betale direkte via MobilePay (brug QR-koden her på siden).",
    },
    {
      question: "Hvad koster det at få hentet et træ?",
      answer: `Prisen er ${pricePerTree} kr per træ.`,
    },    
  ];

  const progressParts = progress && [
    { title: "Opsamlet", amount: progress.collected, color: "#16a34a" },
    { title: "Fejlet", amount: progress.unresolved, color: "#dc2626" },
    { title: "Tildelt", amount: progress.assigned, color: "#2563eb" },
    { title: "Mangler", amount: progress.unassigned, color: "#ffffff" },
    { title: "Ekstra", amount: progress.extra, color: "#f59e0b" },
  ];

  return (
    <div className="flex flex-col gap-4 p-4">
      <div>
        <h3 className="text-lg font-medium mb-2">MobilePay QR-Kode</h3>
        <div className="flex flex-col items-center gap-2">
          <a href={MOBILEPAY_URL} target="_blank" rel="noopener noreferrer">
            <img
              src={apiUrl("/mobilepay.png")}
              alt="MobilePay QR-kode"
              className="h-48 w-48 max-w-full"
            />
          </a>
          <p className="text-sm text-gray-600 text-center">
            Scan QR-koden med MobilePay - er du på mobilen, kan du trykke på
            koden for at åbne MobilePay.
          </p>
        </div>
      </div>

      <div>
        <h3 className="text-lg font-medium mb-2">Spørgsmål og svar</h3>
        <dl className="flex flex-col gap-3">
          {faq.map((item) => (
            <div key={item.question}>
              <dt className="font-medium">{item.question}</dt>
              <dd className="text-sm text-gray-700">{item.answer}</dd>
            </div>
          ))}
        </dl>
      </div>

      {progressParts && progress && progress.total > 0 && (
        <div>
          <h3 className="text-lg font-medium mb-2">Status</h3>
          <div className="flex flex-col items-center gap-3">
            {/* Slice labels in ProgressPie sit at a fixed 80px radius, so keep the pie at ~224px. */}
            <div className="w-56">
              <ProgressPie parts={progressParts} total={progress.total} />
            </div>
            <ul className="text-sm flex flex-wrap justify-center gap-x-4 gap-y-1">
              {progressParts.map((p) => (
                <li key={p.title} className="flex items-center gap-2">
                  <span
                    className="inline-block w-3 h-3 rounded-sm border border-gray-500"
                    style={{ backgroundColor: p.color }}
                  />
                  {p.title}: {p.amount}
                </li>
              ))}
              <li className="font-medium">I alt: {progress.total}</li>
            </ul>
          </div>
        </div>
      )}
    </div>
  );
}
