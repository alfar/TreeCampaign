import { useEffect } from "react";
import { createPortal } from "react-dom";
import Button from "./Button";

interface ConfirmDialogProps {
  open: boolean;
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  confirmClassName?: string;
  onConfirm: () => void;
  onCancel: () => void;
}

export default function ConfirmDialog({
  open,
  title,
  message,
  confirmLabel = "Fortsæt",
  cancelLabel = "Afbryd",
  confirmClassName = "",
  onConfirm,
  onCancel,
}: ConfirmDialogProps) {
  useEffect(() => {
    if (!open) return;
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") onCancel();
    };
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [open, onCancel]);

  if (!open) return null;

  return createPortal(
    <div
      className="fixed inset-0 z-2000 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4"
      onClick={onCancel}
      role="presentation"
    >
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby="confirm-dialog-title"
        className="w-full max-w-md rounded-2xl bg-white p-6 shadow-2xl"
        onClick={(e) => e.stopPropagation()}
      >
        <h2 id="confirm-dialog-title" className="text-xl font-bold text-gray-900">
          {title}
        </h2>
        <p className="mt-3 text-base text-gray-700">{message}</p>
        <div className="mt-6 flex gap-3">
          <Button size="lg" variant="secondary" className="flex-1" onClick={onCancel}>
            {cancelLabel}
          </Button>
          <Button size="lg" className={`flex-1 ${confirmClassName}`} onClick={onConfirm}>
            {confirmLabel}
          </Button>
        </div>
      </div>
    </div>,
    document.body,
  );
}
