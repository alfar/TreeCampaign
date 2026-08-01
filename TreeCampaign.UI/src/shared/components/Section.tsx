import { ChevronDownIcon, ChevronUpIcon } from "@heroicons/react/24/outline";
import { useState } from "react";

interface SectionProps {
  icon: React.ReactNode;
  title: React.ReactNode;
  actions?: React.ReactNode;
  defaultExpanded?: boolean;
  children: React.ReactNode;
}

export default function Section({ icon, title, actions, defaultExpanded = true, children }: SectionProps) {
  const [expanded, setExpanded] = useState(defaultExpanded);

  return (
    <div className="rounded border border-gray-200">
      <div
        className={
          "bg-gray-100 p-2 flex justify-between items-center" +
          (expanded ? " rounded-t-sm" : " rounded")
        }
        onClick={() => setExpanded(!expanded)}
      >
        <div className="flex gap-2 items-center">
          <div className="rounded-full bg-blue-100 p-1">{icon}</div>
          <h2 className="text-lg text-gray-600">{title}</h2>
        </div>
        <div className="flex items-center gap-2">
          {actions && (
            <div onClick={(e) => e.stopPropagation()}>{actions}</div>
          )}
          {expanded ? (
            <ChevronDownIcon className="h-5 w-5" />
          ) : (
            <ChevronUpIcon className="h-5 w-5" />
          )}
        </div>
      </div>
      <div className={expanded ? "rounded-b-sm p-2" : "overflow-hidden max-h-0"}>
        {children}
      </div>
    </div>
  );
}
