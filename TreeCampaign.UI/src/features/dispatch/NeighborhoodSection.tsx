import { BuildingOffice2Icon, ChevronDownIcon, ChevronUpIcon } from "@heroicons/react/24/outline";
import { useState, type PropsWithChildren } from "react";

interface NeighborhoodSectionProps {
    name: string;
}

export default function NeighborhoodSection({ name, children }: NeighborhoodSectionProps & PropsWithChildren )
{
    const [expanded, setExpanded] = useState(true);

    return (
        <div className="rounded border border-gray-200">
            <div className={"bg-gray-100 p-2 flex justify-between" + (expanded ? " rounded-t-sm" : " rounded")} onClick={() => setExpanded(!expanded)}>
                <div className="flex gap-2 items-center"><div className="rounded-full bg-blue-100 p-1"><BuildingOffice2Icon className="h-5 w-5 text-blue-600" /></div><h2 className="text-lg text-gray-600">{name} (6 stop)</h2></div>
                {expanded ? 
                    <ChevronDownIcon className="h-5 w-5" /> :
                    <ChevronUpIcon className="h-5 w-5" />
                }
            </div>
            <div className={expanded ? "rounded-b-sm p-2" : "overflow-hidden max-h-0"}>
                {children}
            </div>
        </div>
    );
}