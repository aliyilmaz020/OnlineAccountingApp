import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import PageMeta from "../../components/common/PageMeta";
import Button from "../../components/ui/button/Button";
import { Modal } from "../../components/ui/modal";
import Alert from "../../components/ui/alert/Alert";
import { useModal } from "../../hooks/useModal";
import { useCompany } from "../../context/CompanyContext";
import { apiPost } from "../../lib/apiClient";
import { ApiError } from "../../lib/apiError";
import type { Company, CreateCompanyRequest } from "../../types/entities";
import CompanyForm from "../Companies/CompanyForm";

export default function SelectCompany() {
  const navigate = useNavigate();
  const { companies, isLoadingCompanies, selectCompany, refreshCompanies } = useCompany();
  const createModal = useModal();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);

  useEffect(() => {
    if (!isLoadingCompanies && companies.length === 1) {
      selectCompany(companies[0].id);
      navigate("/", { replace: true });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isLoadingCompanies, companies]);

  const handleCreate = async (values: CreateCompanyRequest) => {
    setIsSubmitting(true);
    setFormError(null);
    try {
      const created = await apiPost<Company>("/api/Companies/CreateCompany", values);
      await refreshCompanies();
      selectCompany(created.id);
      createModal.closeModal();
      navigate("/", { replace: true });
    } catch (err) {
      setFormError(err instanceof ApiError ? err.message : "Sirket olusturulamadi.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleSelect = (id: string) => {
    selectCompany(id);
    navigate("/", { replace: true });
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-50 p-6 dark:bg-gray-900">
      <PageMeta title="Sirket Sec" description="Calismak istediginiz sirketi secin" />
      <div className="w-full max-w-xl rounded-2xl border border-gray-200 bg-white p-6 dark:border-gray-800 dark:bg-white/[0.03]">
        <h1 className="mb-1 text-xl font-semibold text-gray-800 dark:text-white/90">
          Sirket Secin
        </h1>
        <p className="mb-6 text-sm text-gray-500 dark:text-gray-400">
          Devam etmek icin bir sirket secin veya yeni bir sirket olusturun.
        </p>

        {isLoadingCompanies ? (
          <p className="text-sm text-gray-500">Yukleniyor...</p>
        ) : companies.length === 0 ? (
          <div className="rounded-lg border border-dashed border-gray-300 p-6 text-center dark:border-gray-700">
            <p className="mb-4 text-sm text-gray-500 dark:text-gray-400">
              Henuz bir sirketiniz yok.
            </p>
            <Button onClick={createModal.openModal}>Yeni Sirket Olustur</Button>
          </div>
        ) : (
          <div className="space-y-3">
            {companies.map((company) => (
              <div
                key={company.id}
                className="flex items-center justify-between rounded-lg border border-gray-200 p-4 dark:border-gray-800"
              >
                <div>
                  <p className="font-medium text-gray-800 dark:text-white/90">{company.name}</p>
                  <p className="text-sm text-gray-500 dark:text-gray-400">{company.email}</p>
                </div>
                <Button size="sm" onClick={() => handleSelect(company.id)}>
                  Sec
                </Button>
              </div>
            ))}
            <Button variant="outline" onClick={createModal.openModal}>
              Yeni Sirket Olustur
            </Button>
          </div>
        )}
      </div>

      <Modal isOpen={createModal.isOpen} onClose={createModal.closeModal} className="max-w-lg p-6">
        <h4 className="mb-4 text-lg font-semibold text-gray-800 dark:text-white/90">
          Yeni Sirket
        </h4>
        {formError && (
          <div className="mb-4">
            <Alert variant="error" title="Hata" message={formError} />
          </div>
        )}
        <CompanyForm onSubmit={handleCreate} onCancel={createModal.closeModal} isSubmitting={isSubmitting} />
      </Modal>
    </div>
  );
}
