import { createRoot } from "react-dom/client";
import PublicIntakeForm from "../features/public-intake/PublicIntakeForm";
import { setApiBaseUrl } from "../shared/api/client";
import css from "./embed.css?inline";

// Browsers ignore @property inside a shadow root's stylesheet, which leaves Tailwind's --tw-*
// variables (e.g. --tw-border-style) undefined. Registering them on the document fixes that;
// the registration is global but applies to elements inside shadow trees too.
const PROPERTY_RULES = /@property\s+--[\w-]+\s*\{[^}]*\}/g;
const shadowCss = css.replace(PROPERTY_RULES, "");

function registerCustomProperties() {
  const id = "tree-campaign-intake-properties";
  if (document.getElementById(id)) return;
  const style = document.createElement("style");
  style.id = id;
  style.textContent = (css.match(PROPERTY_RULES) ?? []).join("\n");
  document.head.appendChild(style);
}

export interface MountOptions {
  campaignId: string;
  /** Origin of the TreeCampaign API/host, e.g. "https://trae.example.dk". Defaults to same origin. */
  apiBaseUrl?: string;
}

/**
 * Renders the public intake form into `target`, inside a shadow root so Tailwind's styles
 * and the host page's styles cannot affect each other. Returns a function that unmounts it.
 */
export function mount(target: HTMLElement, { campaignId, apiBaseUrl }: MountOptions): () => void {
  setApiBaseUrl(apiBaseUrl ?? "");

  registerCustomProperties();

  const shadow = target.shadowRoot ?? target.attachShadow({ mode: "open" });
  const style = document.createElement("style");
  style.textContent = shadowCss;
  const container = document.createElement("div");
  shadow.replaceChildren(style, container);

  const root = createRoot(container);
  root.render(<PublicIntakeForm campaignId={campaignId} />);

  return () => {
    root.unmount();
    shadow.replaceChildren();
  };
}
