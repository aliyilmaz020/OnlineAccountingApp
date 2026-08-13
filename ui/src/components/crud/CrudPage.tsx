import { useState } from "react";
import type { ReactNode } from "react";
import type { useCrud } from "../../hooks/useCrud";
import { ApiError } from "../../lib/apiError";
import Button from "../ui/button/Button";
import { Modal } from "../ui/modal";
import Alert from "../ui/alert/Alert";
import Input from "../form/input/InputField";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "../ui/table";
import { useModal } from "../../hooks/useModal";

export interface CrudColumn<T> {
  header: string;
  render: (item: T) => ReactNode;
}

export interface CrudFormRenderProps<TListItem, TCreateReq, TUpdateReq> {
  initial?: TListItem;
  onSubmit: (values: TCreateReq & Partial<TUpdateReq>) => Promise<void>;
  onCancel: () => void;
  isSubmitting: boolean;
}

interface CrudPageProps<TListItem extends { id: string }, TCreateReq, TUpdateReq> {
  title: string;
  crud: ReturnType<typeof useCrud<TListItem, TCreateReq, TUpdateReq>>;
  columns: CrudColumn<TListItem>[];
  renderForm: (props: CrudFormRenderProps<TListItem, TCreateReq, TUpdateReq>) => ReactNode;
}

export default function CrudPage<TListItem extends { id: string }, TCreateReq, TUpdateReq>(
  props: CrudPageProps<TListItem, TCreateReq, TUpdateReq>,
) {
  const { title, crud, columns, renderForm } = props;
  const createModal = useModal();
  const [editingItem, setEditingItem] = useState<TListItem | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);

  const handleCreate = async (values: TCreateReq) => {
    setIsSubmitting(true);
    setFormError(null);
    try {
      await crud.create(values);
      createModal.closeModal();
    } catch (err) {
      setFormError(err instanceof ApiError ? err.message : "Kaydedilemedi.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleUpdate = async (values: TUpdateReq) => {
    if (!editingItem) return;
    setIsSubmitting(true);
    setFormError(null);
    try {
      await crud.update(editingItem.id, values);
      setEditingItem(null);
    } catch (err) {
      setFormError(err instanceof ApiError ? err.message : "Guncellenemedi.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (item: TListItem) => {
    const confirmed = window.confirm("Bu kaydi silmek istediginize emin misiniz?");
    if (!confirmed) return;
    try {
      await crud.remove(item.id);
    } catch (err) {
      window.alert(err instanceof ApiError ? err.message : "Silinemedi.");
    }
  };

  return (
    <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03]">
      <div className="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <h3 className="text-lg font-semibold text-gray-800 dark:text-white/90">{title}</h3>
        <div className="flex items-center gap-3">
          <Input
            placeholder="Ara..."
            value={crud.searchTerm}
            onChange={(e) => crud.setSearchTerm(e.target.value)}
          />
          <Button
            size="sm"
            onClick={() => {
              setFormError(null);
              createModal.openModal();
            }}
          >
            Yeni Ekle
          </Button>
        </div>
      </div>

      {crud.error && (
        <div className="mb-4">
          <Alert variant="error" title="Hata" message={crud.error} />
        </div>
      )}

      <div className="overflow-x-auto">
        <Table>
          <TableHeader className="border-b border-gray-100 dark:border-white/[0.05]">
            <TableRow>
              {columns.map((col) => (
                <TableCell
                  key={col.header}
                  isHeader
                  className="px-4 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  {col.header}
                </TableCell>
              ))}
              <TableCell
                isHeader
                className="px-4 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-400"
              >
                Islemler
              </TableCell>
            </TableRow>
          </TableHeader>
          <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
            {crud.isLoading ? (
              <TableRow>
                <TableCell className="px-4 py-4 text-gray-500" colSpan={columns.length + 1}>
                  Yukleniyor...
                </TableCell>
              </TableRow>
            ) : crud.items.length === 0 ? (
              <TableRow>
                <TableCell className="px-4 py-4 text-gray-500" colSpan={columns.length + 1}>
                  Kayit bulunamadi.
                </TableCell>
              </TableRow>
            ) : (
              crud.items.map((item) => (
                <TableRow key={item.id}>
                  {columns.map((col) => (
                    <TableCell key={col.header} className="px-4 py-3 text-sm text-gray-700 dark:text-gray-300">
                      {col.render(item)}
                    </TableCell>
                  ))}
                  <TableCell className="px-4 py-3 text-sm">
                    <div className="flex items-center gap-3">
                      <button
                        className="text-brand-500 hover:text-brand-600"
                        onClick={() => {
                          setFormError(null);
                          setEditingItem(item);
                        }}
                      >
                        Duzenle
                      </button>
                      <button
                        className="text-error-500 hover:text-error-600"
                        onClick={() => handleDelete(item)}
                      >
                        Sil
                      </button>
                    </div>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>

      <div className="mt-4 flex items-center justify-between text-sm text-gray-500 dark:text-gray-400">
        <span>Toplam {crud.totalCount} kayit</span>
        <div className="flex items-center gap-2">
          <Button
            size="sm"
            variant="outline"
            disabled={crud.pageNumber <= 1}
            onClick={() => crud.setPageNumber(crud.pageNumber - 1)}
          >
            Onceki
          </Button>
          <span>
            Sayfa {crud.pageNumber} / {Math.max(crud.totalPages, 1)}
          </span>
          <Button
            size="sm"
            variant="outline"
            disabled={crud.pageNumber >= crud.totalPages}
            onClick={() => crud.setPageNumber(crud.pageNumber + 1)}
          >
            Sonraki
          </Button>
        </div>
      </div>

      <Modal isOpen={createModal.isOpen} onClose={createModal.closeModal} className="max-w-lg p-6">
        <h4 className="mb-4 text-lg font-semibold text-gray-800 dark:text-white/90">Yeni {title}</h4>
        {formError && (
          <div className="mb-4">
            <Alert variant="error" title="Hata" message={formError} />
          </div>
        )}
        {renderForm({
          onSubmit: handleCreate as (values: TCreateReq & Partial<TUpdateReq>) => Promise<void>,
          onCancel: createModal.closeModal,
          isSubmitting,
        })}
      </Modal>

      <Modal isOpen={editingItem !== null} onClose={() => setEditingItem(null)} className="max-w-lg p-6">
        <h4 className="mb-4 text-lg font-semibold text-gray-800 dark:text-white/90">{title} Duzenle</h4>
        {formError && (
          <div className="mb-4">
            <Alert variant="error" title="Hata" message={formError} />
          </div>
        )}
        {editingItem &&
          renderForm({
            initial: editingItem,
            onSubmit: handleUpdate as (values: TCreateReq & Partial<TUpdateReq>) => Promise<void>,
            onCancel: () => setEditingItem(null),
            isSubmitting,
          })}
      </Modal>
    </div>
  );
}
