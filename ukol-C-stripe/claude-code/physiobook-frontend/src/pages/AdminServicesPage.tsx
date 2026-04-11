import React, { useState } from 'react';
import { useToast } from '@/contexts/ToastContext';
import {
  useServices,
  useCreateService,
  useUpdateService,
  useDeleteService,
} from '@/features/services/hooks';
import type { Service } from '@/features/services/types';
import Layout from '@/components/Layout';

export default function AdminServicesPage() {
  const [page, setPage] = useState(1);
  const pageSize = 20;
  const { data, loading, error, refetch } = useServices(page, pageSize);
  const { createService, loading: creating } = useCreateService();
  const { updateService, loading: updating } = useUpdateService();
  const { deleteService, loading: deleting } = useDeleteService();
  const { showToast } = useToast();

  // Create form state
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [createName, setCreateName] = useState('');
  const [createDescription, setCreateDescription] = useState('');
  const [createDuration, setCreateDuration] = useState('');
  const [createPrice, setCreatePrice] = useState('');
  const [createErrors, setCreateErrors] = useState<Record<string, string>>({});

  // Edit state
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editName, setEditName] = useState('');
  const [editDescription, setEditDescription] = useState('');
  const [editDuration, setEditDuration] = useState('');
  const [editPrice, setEditPrice] = useState('');
  const [editErrors, setEditErrors] = useState<Record<string, string>>({});

  // Delete confirm
  const [deleteConfirmId, setDeleteConfirmId] = useState<string | null>(null);

  const validateCreate = (): boolean => {
    const errs: Record<string, string> = {};
    if (!createName.trim()) errs.name = 'Nazev je povinny';
    if (!createDescription.trim()) errs.description = 'Popis je povinny';
    if (!createDuration || Number(createDuration) <= 0) errs.duration = 'Delka musi byt kladne cislo';
    if (!createPrice || Number(createPrice) < 0) errs.price = 'Cena musi byt nezaporne cislo';
    setCreateErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const validateEdit = (): boolean => {
    const errs: Record<string, string> = {};
    if (!editName.trim()) errs.name = 'Nazev je povinny';
    if (!editDescription.trim()) errs.description = 'Popis je povinny';
    if (!editDuration || Number(editDuration) <= 0) errs.duration = 'Delka musi byt kladne cislo';
    if (!editPrice || Number(editPrice) < 0) errs.price = 'Cena musi byt nezaporne cislo';
    setEditErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateCreate()) return;

    const result = await createService({
      name: createName.trim(),
      description: createDescription.trim(),
      durationMinutes: Number(createDuration),
      price: Number(createPrice),
    });

    if (result) {
      showToast('Sluzba byla uspesne vytvorena.', 'success');
      setCreateName('');
      setCreateDescription('');
      setCreateDuration('');
      setCreatePrice('');
      setCreateErrors({});
      setShowCreateForm(false);
      refetch();
    } else {
      showToast('Nepodarilo se vytvorit sluzbu.', 'error');
    }
  };

  const startEdit = (service: Service) => {
    setEditingId(service.id);
    setEditName(service.name);
    setEditDescription(service.description);
    setEditDuration(String(service.durationMinutes));
    setEditPrice(String(service.price));
    setEditErrors({});
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingId || !validateEdit()) return;

    const result = await updateService(editingId, {
      name: editName.trim(),
      description: editDescription.trim(),
      durationMinutes: Number(editDuration),
      price: Number(editPrice),
    });

    if (result) {
      showToast('Sluzba byla uspesne aktualizovana.', 'success');
      setEditingId(null);
      refetch();
    } else {
      showToast('Nepodarilo se aktualizovat sluzbu.', 'error');
    }
  };

  const handleDelete = async () => {
    if (!deleteConfirmId) return;
    const success = await deleteService(deleteConfirmId);
    if (success) {
      showToast('Sluzba byla uspesne smazana.', 'success');
      refetch();
    } else {
      showToast('Nepodarilo se smazat sluzbu.', 'error');
    }
    setDeleteConfirmId(null);
  };

  return (
    <Layout>
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Sprava sluzeb</h1>
          <p className="text-gray-500 mt-1">Spravujte nabidku sluzeb</p>
        </div>
        <button
          onClick={() => setShowCreateForm(!showCreateForm)}
          className="inline-flex items-center gap-2 px-5 py-2.5 bg-teal-600 text-white font-medium rounded-lg hover:bg-teal-700 transition-colors shadow-sm"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-5 w-5"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fillRule="evenodd"
              d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z"
              clipRule="evenodd"
            />
          </svg>
          Nova sluzba
        </button>
      </div>

      {/* Create Form */}
      {showCreateForm && (
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 mb-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Vytvorit novou sluzbu</h2>
          <form onSubmit={handleCreate} className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label htmlFor="create-name" className="block text-sm font-medium text-gray-700 mb-1">
                  Nazev
                </label>
                <input
                  id="create-name"
                  type="text"
                  value={createName}
                  onChange={(e) => setCreateName(e.target.value)}
                  className={`w-full px-4 py-2.5 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-shadow ${
                    createErrors.name ? 'border-red-300 bg-red-50' : 'border-gray-300'
                  }`}
                  placeholder="Nazev sluzby"
                />
                {createErrors.name && <p className="mt-1 text-sm text-red-600">{createErrors.name}</p>}
              </div>
              <div>
                <label htmlFor="create-description" className="block text-sm font-medium text-gray-700 mb-1">
                  Popis
                </label>
                <input
                  id="create-description"
                  type="text"
                  value={createDescription}
                  onChange={(e) => setCreateDescription(e.target.value)}
                  className={`w-full px-4 py-2.5 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-shadow ${
                    createErrors.description ? 'border-red-300 bg-red-50' : 'border-gray-300'
                  }`}
                  placeholder="Popis sluzby"
                />
                {createErrors.description && (
                  <p className="mt-1 text-sm text-red-600">{createErrors.description}</p>
                )}
              </div>
              <div>
                <label htmlFor="create-duration" className="block text-sm font-medium text-gray-700 mb-1">
                  Delka (minuty)
                </label>
                <input
                  id="create-duration"
                  type="number"
                  min="1"
                  value={createDuration}
                  onChange={(e) => setCreateDuration(e.target.value)}
                  className={`w-full px-4 py-2.5 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-shadow ${
                    createErrors.duration ? 'border-red-300 bg-red-50' : 'border-gray-300'
                  }`}
                  placeholder="30"
                />
                {createErrors.duration && (
                  <p className="mt-1 text-sm text-red-600">{createErrors.duration}</p>
                )}
              </div>
              <div>
                <label htmlFor="create-price" className="block text-sm font-medium text-gray-700 mb-1">
                  Cena (Kc)
                </label>
                <input
                  id="create-price"
                  type="number"
                  min="0"
                  value={createPrice}
                  onChange={(e) => setCreatePrice(e.target.value)}
                  className={`w-full px-4 py-2.5 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-shadow ${
                    createErrors.price ? 'border-red-300 bg-red-50' : 'border-gray-300'
                  }`}
                  placeholder="500"
                />
                {createErrors.price && <p className="mt-1 text-sm text-red-600">{createErrors.price}</p>}
              </div>
            </div>
            <div className="flex justify-end gap-3 pt-2">
              <button
                type="button"
                onClick={() => {
                  setShowCreateForm(false);
                  setCreateErrors({});
                }}
                className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
              >
                Zrusit
              </button>
              <button
                type="submit"
                disabled={creating}
                className="px-6 py-2 text-sm font-medium text-white bg-teal-600 rounded-lg hover:bg-teal-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                {creating ? (
                  <span className="inline-flex items-center gap-2">
                    <svg
                      className="animate-spin h-4 w-4"
                      xmlns="http://www.w3.org/2000/svg"
                      fill="none"
                      viewBox="0 0 24 24"
                    >
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                      <path
                        className="opacity-75"
                        fill="currentColor"
                        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                      />
                    </svg>
                    Vytvarim...
                  </span>
                ) : (
                  'Vytvorit'
                )}
              </button>
            </div>
          </form>
        </div>
      )}

      {/* Loading */}
      {loading && (
        <div className="flex justify-center items-center py-20">
          <svg
            className="animate-spin h-8 w-8 text-teal-600"
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
          >
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
            <path
              className="opacity-75"
              fill="currentColor"
              d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
            />
          </svg>
        </div>
      )}

      {error && (
        <div className="p-6 bg-red-50 border border-red-200 rounded-xl text-red-700 text-center">
          <p className="font-medium">Nepodarilo se nacist sluzby</p>
          <p className="text-sm mt-1">{error}</p>
        </div>
      )}

      {/* Services Table */}
      {!loading && !error && data && (
        <>
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="bg-gray-50 border-b border-gray-200">
                    <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">
                      Nazev
                    </th>
                    <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider hidden md:table-cell">
                      Popis
                    </th>
                    <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">
                      Delka
                    </th>
                    <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">
                      Cena
                    </th>
                    <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">
                      Stav
                    </th>
                    <th className="text-right px-6 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">
                      Akce
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200">
                  {data.items.map((service) => (
                    <React.Fragment key={service.id}>
                      {editingId === service.id ? (
                        <tr className="bg-teal-50">
                          <td colSpan={6} className="px-6 py-4">
                            <form onSubmit={handleUpdate} className="space-y-4">
                              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                <div>
                                  <label className="block text-sm font-medium text-gray-700 mb-1">Nazev</label>
                                  <input
                                    type="text"
                                    value={editName}
                                    onChange={(e) => setEditName(e.target.value)}
                                    className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent text-sm ${
                                      editErrors.name ? 'border-red-300 bg-red-50' : 'border-gray-300'
                                    }`}
                                  />
                                  {editErrors.name && (
                                    <p className="mt-1 text-xs text-red-600">{editErrors.name}</p>
                                  )}
                                </div>
                                <div>
                                  <label className="block text-sm font-medium text-gray-700 mb-1">Popis</label>
                                  <input
                                    type="text"
                                    value={editDescription}
                                    onChange={(e) => setEditDescription(e.target.value)}
                                    className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent text-sm ${
                                      editErrors.description ? 'border-red-300 bg-red-50' : 'border-gray-300'
                                    }`}
                                  />
                                  {editErrors.description && (
                                    <p className="mt-1 text-xs text-red-600">{editErrors.description}</p>
                                  )}
                                </div>
                                <div>
                                  <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Delka (minuty)
                                  </label>
                                  <input
                                    type="number"
                                    min="1"
                                    value={editDuration}
                                    onChange={(e) => setEditDuration(e.target.value)}
                                    className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent text-sm ${
                                      editErrors.duration ? 'border-red-300 bg-red-50' : 'border-gray-300'
                                    }`}
                                  />
                                  {editErrors.duration && (
                                    <p className="mt-1 text-xs text-red-600">{editErrors.duration}</p>
                                  )}
                                </div>
                                <div>
                                  <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Cena (Kc)
                                  </label>
                                  <input
                                    type="number"
                                    min="0"
                                    value={editPrice}
                                    onChange={(e) => setEditPrice(e.target.value)}
                                    className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent text-sm ${
                                      editErrors.price ? 'border-red-300 bg-red-50' : 'border-gray-300'
                                    }`}
                                  />
                                  {editErrors.price && (
                                    <p className="mt-1 text-xs text-red-600">{editErrors.price}</p>
                                  )}
                                </div>
                              </div>
                              <div className="flex justify-end gap-2">
                                <button
                                  type="button"
                                  onClick={() => setEditingId(null)}
                                  className="px-3 py-1.5 text-sm font-medium text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                                >
                                  Zrusit
                                </button>
                                <button
                                  type="submit"
                                  disabled={updating}
                                  className="px-4 py-1.5 text-sm font-medium text-white bg-teal-600 rounded-lg hover:bg-teal-700 disabled:opacity-50 transition-colors"
                                >
                                  {updating ? 'Ukladam...' : 'Ulozit'}
                                </button>
                              </div>
                            </form>
                          </td>
                        </tr>
                      ) : (
                        <tr className="hover:bg-gray-50 transition-colors">
                          <td className="px-6 py-4 text-sm font-medium text-gray-900">{service.name}</td>
                          <td className="px-6 py-4 text-sm text-gray-600 hidden md:table-cell max-w-xs truncate">
                            {service.description}
                          </td>
                          <td className="px-6 py-4 text-sm text-gray-600">{service.durationMinutes} min</td>
                          <td className="px-6 py-4 text-sm font-medium text-teal-700">{service.price} Kc</td>
                          <td className="px-6 py-4">
                            <span
                              className={`inline-flex items-center px-2.5 py-0.5 text-xs font-medium rounded-full ${
                                service.isActive
                                  ? 'bg-green-100 text-green-800'
                                  : 'bg-gray-100 text-gray-500'
                              }`}
                            >
                              {service.isActive ? 'Aktivni' : 'Neaktivni'}
                            </span>
                          </td>
                          <td className="px-6 py-4 text-right">
                            <div className="flex justify-end gap-2">
                              <button
                                onClick={() => startEdit(service)}
                                className="px-3 py-1.5 text-sm font-medium text-blue-600 bg-blue-50 border border-blue-200 rounded-lg hover:bg-blue-100 transition-colors"
                              >
                                Upravit
                              </button>
                              <button
                                onClick={() => setDeleteConfirmId(service.id)}
                                className="px-3 py-1.5 text-sm font-medium text-red-600 bg-red-50 border border-red-200 rounded-lg hover:bg-red-100 transition-colors"
                              >
                                Smazat
                              </button>
                            </div>
                          </td>
                        </tr>
                      )}
                    </React.Fragment>
                  ))}
                </tbody>
              </table>
            </div>
          </div>

          {/* Pagination */}
          {data.totalPages > 1 && (
            <div className="flex justify-center items-center gap-2 mt-6">
              <button
                onClick={() => setPage((p) => Math.max(1, p - 1))}
                disabled={page === 1}
                className="px-4 py-2 text-sm font-medium rounded-lg border border-gray-300 text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                Predchozi
              </button>
              {Array.from({ length: data.totalPages }, (_, i) => i + 1).map((p) => (
                <button
                  key={p}
                  onClick={() => setPage(p)}
                  className={`px-4 py-2 text-sm font-medium rounded-lg transition-colors ${
                    p === page
                      ? 'bg-teal-600 text-white'
                      : 'border border-gray-300 text-gray-700 hover:bg-gray-50'
                  }`}
                >
                  {p}
                </button>
              ))}
              <button
                onClick={() => setPage((p) => Math.min(data.totalPages, p + 1))}
                disabled={page === data.totalPages}
                className="px-4 py-2 text-sm font-medium rounded-lg border border-gray-300 text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                Dalsi
              </button>
            </div>
          )}
        </>
      )}

      {/* Delete Confirmation Modal */}
      {deleteConfirmId && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
          <div
            className="absolute inset-0 bg-black/50"
            onClick={() => setDeleteConfirmId(null)}
          />
          <div className="relative bg-white rounded-xl shadow-xl p-6 max-w-sm w-full">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Smazat sluzbu</h3>
            <p className="text-gray-600 text-sm mb-6">
              Opravdu chcete smazat tuto sluzbu? Sluzba bude deaktivovana.
            </p>
            <div className="flex justify-end gap-3">
              <button
                onClick={() => setDeleteConfirmId(null)}
                className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
              >
                Zpet
              </button>
              <button
                onClick={handleDelete}
                disabled={deleting}
                className="px-4 py-2 text-sm font-medium text-white bg-red-600 rounded-lg hover:bg-red-700 disabled:opacity-50 transition-colors"
              >
                {deleting ? 'Mazani...' : 'Smazat'}
              </button>
            </div>
          </div>
        </div>
      )}
    </Layout>
  );
}
