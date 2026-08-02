import { useEffect, useState } from "react";
import { getUsers } from "../../shared/api/client";
import type { User } from "../../shared/api/models/user";
import CreateUserForm from "./CreateUserForm";
import NavigationPage from "../../shared/components/NavigationPage";

export default function UserManagementScreen() {
  const [users, setUsers] = useState<User[]>([]);
  const [showCreateForm, setShowCreateForm] = useState(false);

  useEffect(() => {
    getUsers().then(setUsers);
  }, []);

  const handleCreated = (user: User) => {
    setUsers((prev) => [...prev, user]);
    setShowCreateForm(false);
  };

  return (
    <NavigationPage>
      <div className="p-4 space-y-4">
        <div className="flex items-center justify-between">
          <h1 className="text-xl font-bold">Brugere</h1>
          <button
            onClick={() => setShowCreateForm((v) => !v)}
            className="text-sm bg-blue-600 text-white py-1.5 px-4 rounded"
          >
            {showCreateForm ? "Annuller" : "Ny bruger"}
          </button>
        </div>
        {showCreateForm && <CreateUserForm onCreated={handleCreated} />}
        {users.map((u) => (
          <div key={u.id} className="p-4 border rounded">
            <p className="font-semibold">{u.displayName}</p>
            <p className="text-sm text-gray-500">{u.email}</p>
          </div>
        ))}
      </div>
    </NavigationPage>
  );
}
