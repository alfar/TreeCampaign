import { ChevronDownIcon, ChevronUpIcon } from "@heroicons/react/24/outline";
import { useEffect, useRef, useState, type PropsWithChildren } from "react";

interface FilterPopoverProps {
  label: string;
}

/** A summary button that opens a floating box over the page (no layout shift). */
export default function FilterPopover({
  label,
  children,
}: PropsWithChildren<FilterPopoverProps>) {
  const [open, setOpen] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!open) return;
    const onPointerDown = (e: MouseEvent) => {
      if (!containerRef.current?.contains(e.target as Node)) setOpen(false);
    };
    const onKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") setOpen(false);
    };
    document.addEventListener("mousedown", onPointerDown);
    document.addEventListener("keydown", onKeyDown);
    return () => {
      document.removeEventListener("mousedown", onPointerDown);
      document.removeEventListener("keydown", onKeyDown);
    };
  }, [open]);

  return (
    <div ref={containerRef} className="relative">
      <button
        type="button"
        onClick={() => setOpen((v) => !v)}
        aria-expanded={open}
        className="flex items-center gap-1 text-sm text-gray-600 hover:text-gray-900"
      >
        {open ? (
          <ChevronUpIcon className="h-4 w-4" />
        ) : (
          <ChevronDownIcon className="h-4 w-4" />
        )}
        {label}
      </button>
      {open && (
        <div className="absolute left-0 top-full z-20 flex flex-col gap-2 rounded-lg border border-gray-300 bg-white p-3 shadow-lg whitespace-nowrap">
          {children}
        </div>
      )}
    </div>
  );
}
