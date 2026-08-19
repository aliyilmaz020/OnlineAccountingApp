import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { Modal } from "../../components/ui/modal";
import Alert from "../../components/ui/alert/Alert";
import Button from "../../components/ui/button/Button";
import Checkbox from "../../components/form/input/Checkbox";
import { apiDelete, apiGet, apiPost } from "../../lib/apiClient";
import { ApiError } from "../../lib/apiError";
import type { PagedResult } from "../../types/api";
import type { Role, UserListItem } from "../../types/entities";

interface Props {
  user: UserListItem;
  onClose: () => void;
}

export default function ManageUserRolesModal({ user, onClose }: Props) {
  const { t } = useTranslation(["users", "common", "crud"]);
  const [allRoles, setAllRoles] = useState<Role[]>([]);
  const [assignedRoleNames, setAssignedRoleNames] = useState<Set<string>>(new Set());
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pendingRoleName, setPendingRoleName] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setIsLoading(true);
    setError(null);
    Promise.all([
      apiGet<PagedResult<Role>>("/api/Roles/GetRoles", { pageNumber: 1, pageSize: 100 }),
      apiGet<Role[]>(`/api/Roles/GetUserRoles/${user.id}`),
    ])
      .then(([rolesResult, userRoles]) => {
        if (cancelled) return;
        setAllRoles(rolesResult.items);
        setAssignedRoleNames(new Set(userRoles.map((r) => r.name)));
      })
      .catch((err) => {
        if (!cancelled) setError(err instanceof ApiError ? err.message : t("users:modal.saveFailed"));
      })
      .finally(() => {
        if (!cancelled) setIsLoading(false);
      });
    return () => {
      cancelled = true;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user.id]);

  const toggleRole = async (role: Role, checked: boolean) => {
    setPendingRoleName(role.name);
    setError(null);
    try {
      if (checked) {
        await apiPost("/api/Roles/AssignRoleToUser", { userId: user.id, roleCode: role.code });
      } else {
        await apiDelete("/api/Roles/RemoveRoleFromUser", { userId: user.id, roleName: role.name });
      }
      setAssignedRoleNames((prev) => {
        const next = new Set(prev);
        if (checked) {
          next.add(role.name);
        } else {
          next.delete(role.name);
        }
        return next;
      });
    } catch (err) {
      setError(err instanceof ApiError ? err.message : t("users:modal.saveFailed"));
    } finally {
      setPendingRoleName(null);
    }
  };

  return (
    <Modal isOpen onClose={onClose} className="max-w-lg p-6">
      <h4 className="mb-4 text-lg font-semibold text-gray-800 dark:text-white/90">
        {t("users:modal.title", { userName: user.userName ?? user.email ?? user.id })}
      </h4>

      {error && (
        <div className="mb-4">
          <Alert variant="error" title={t("crud:errorTitle")} message={error} />
        </div>
      )}

      {isLoading ? (
        <p className="text-sm text-gray-500 dark:text-gray-400">{t("users:modal.loading")}</p>
      ) : allRoles.length === 0 ? (
        <p className="text-sm text-gray-500 dark:text-gray-400">{t("users:modal.noRoles")}</p>
      ) : (
        <div className="space-y-3">
          {allRoles.map((role) => (
            <Checkbox
              key={role.id}
              label={`${role.name} (${role.code})`}
              checked={assignedRoleNames.has(role.name)}
              disabled={pendingRoleName === role.name}
              onChange={(checked) => toggleRole(role, checked)}
            />
          ))}
        </div>
      )}

      <div className="flex justify-end pt-6">
        <Button type="button" variant="outline" onClick={onClose}>
          {t("common:cancel")}
        </Button>
      </div>
    </Modal>
  );
}
