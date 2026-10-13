import { useParams } from "react-router-dom";
import PublicIntakeForm from "./PublicIntakeForm";

export default function PublicIntakePage() {
  const { campaignId } = useParams();

  if (!campaignId) {
    return null;
  }

  return <PublicIntakeForm campaignId={campaignId} />;
}
