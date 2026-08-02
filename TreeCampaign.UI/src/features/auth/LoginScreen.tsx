import { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../../app/AuthContext";

export default function LoginScreen() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const from = (location.state as { from?: Location })?.from?.pathname ?? "/";

  const handleSubmit = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    if (isSubmitting) return;

    setIsSubmitting(true);
    setError(null);
    try {
      const success = await login(email, password);
      if (success) {
        navigate(from, { replace: true });
      } else {
        setError("Forkert email eller adgangskode.");
      }
    } catch {
      setError("Noget gik galt. Prøv igen.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-svh bg-blue-800">
      <form
        onSubmit={handleSubmit}
        className="flex flex-col gap-4 p-8 border rounded bg-white w-full max-w-sm"
      >
        <div className="rounded-full bg-blue-600 w-20 h-20 flex items-center justify-center self-center mb-4">
        <img src="/logo.png" alt="TreeCampaign logo" className="w-12 self-center" />
        </div>
        <h1 className="text-lg font-semibold text-center">Log ind</h1>
        <div>
          <label className="text-sm font-medium text-gray-700 block mb-1">Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
            autoFocus
            required
          />
        </div>
        <div>
          <label className="text-sm font-medium text-gray-700 block mb-1">Adgangskode</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full border rounded px-3 py-2 text-sm"
            required
          />
        </div>
        {error && <p className="text-sm text-red-600">{error}</p>}
        <button
          type="submit"
          disabled={isSubmitting}
          className="bg-blue-600 text-white py-2 px-5 rounded disabled:opacity-40"
        >
          {isSubmitting ? "Logger ind…" : "Log ind"}
        </button>
        <Link to="/register" className="text-sm text-blue-600 text-center hover:underline">
          Opret en ny gruppe
        </Link>
      </form>
    </div>
  );
}
