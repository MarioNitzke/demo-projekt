import React, { useState, useMemo } from 'react';
import { useToast } from '@/contexts/ToastContext';
import {
  useTimeSlots,
  useCreateTimeSlot,
  useUpdateTimeSlot,
  useDeleteTimeSlot,
} from '@/features/timeslots/hooks';
import type { TimeSlot } from '@/features/timeslots/types';
import Layout from '@/components/Layout';

const DAY_NAMES: Record<number, string> = {
  0: 'Nedele',
  1: 'Pondeli',
  2: 'Utery',
  3: 'Streda',
  4: 'Ctvrtek',
  5: 'Patek',
  6: 'Sobota',
};

const ORDERED_DAYS = [1, 2, 3, 4, 5, 6, 0]; // Monday first

export default function AdminTimeSlotsPage() {
  const { data: timeSlots, loading, error, refetch } = useTimeSlots();
  const { createTimeSlot, loading: creating } = useCreateTimeSlot();
  const { updateTimeSlot, loading: updating } = useUpdateTimeSlot();
  const { deleteTimeSlot, loading: deletingSlot } = useDeleteTimeSlot();
  const { showToast } = useToast();

  // Create form state
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [createDay, setCreateDay] = useState(1);
  const [createStart, setCreateStart] = useState('');
  const [createEnd, setCreateEnd] = useState('');
  const [createAvailable, setCreateAvailable] = useState(true);
  const [createErrors, setCreateErrors] = useState<Record<string, string>>({});

  // Edit state
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editDay, setEditDay] = useState(1);
  const [editStart, setEditStart] = useState('');
  const [editEnd, setEditEnd] = useState('');
  const [editAvailable, setEditAvailable] = useState(true);
  const [editErrors, setEditErrors] = useState<Record<string, string>>({});

  // Delete confirm
  const [deleteConfirmId, setDeleteConfirmId] = useState<string | null>(null);

  const groupedSlots = useMemo(() => {
    if (!timeSlots) return {};
    const groups: Record<number, TimeSlot[]> = {};
    timeSlots.forEach((slot) => {
      if (!groups[slot.dayOfWeek]) groups[slot.dayOfWeek] = [];
      groups[slot.dayOfWeek].push(slot);
    });
    // Sort each day's slots by start time
    Object.values(groups).forEach((slots) =>
      slots.sort((a, b) => a.startTime.localeCompare(b.startTime)),
    );
    return groups;
  }, [timeSlots]);

  const validateCreate = (): boolean => {
    const errs: Record<string, string> = {};
    if (!createStart) errs.start = 'Cas zacatku je povinny';
    if (!createEnd) errs.end = 'Cas konce je povinny';
    if (createStart && createEnd && createStart >= createEnd)
      errs.end = 'Cas konce musi byt po casu zacatku';
    setCreateErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const validateEdit = (): boolean => {
    const errs: Record<string, string> = {};
    if (!editStart) errs.start = 'Cas zacatku je povinny';
    if (!editEnd) errs.end = 'Cas konce je povinny';
    if (editStart && editEnd && editStart >= editEnd)
      errs.end = 'Cas konce musi byt po casu zacatku';
    setEditErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateCreate()) return;

    const result = await createTimeSlot({
      dayOfWeek: createDay,
      startTime: createStart,
      endTime: createEnd,
      isAvailable: createAvailable,
    });

    if (result) {
      showToast('Casovy slot byl uspesne vytvoren.', 'success');
      setCreateStart('');
      setCreateEnd('');
      setCreateAvailable(true);
      setCreateErrors({});
      setShowCreateForm(false);
      refetch();
    } else {
      showToast('Nepodarilo se vytvorit casovy slot.', 'error');
    }
  };

  const startEdit = (slot: TimeSlot) => {
    setEditingId(slot.id);
    setEditDay(slot.dayOfWeek);
    setEditStart(slot.startTime);
    setEditEnd(slot.endTime);
    setEditAvailable(slot.isAvailable);
    setEditErrors({});
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingId || !validateEdit()) return;

    const result = await updateTimeSlot(editingId, {
      dayOfWeek: editDay,
      startTime: editStart,
      endTime: editEnd,
      isAvailable: editAvailable,
    });

    if (result) {
      showToast('Casovy slot byl uspesne aktualizovan.', 'success');
      setEditingId(null);
      refetch();
    } else {
      showToast('Nepodarilo se aktualizovat casovy slot.', 'error');
    }
  };

  const handleDelete = async () => {
    if (!deleteConfirmId) return;
    const success = await deleteTimeSlot(deleteConfirmId);
    if (success) {
      showToast('Casovy slot byl uspesne smazan.', 'success');
      refetch();
    } else {
      showToast('Nepodarilo se smazat casovy slot.', 'error');
    }
    setDeleteConfirmId(null);
  };

  return (
    <Layout>
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Sprava casovych slotu</h1>
          <p className="text-gray-500 mt-1">Nastavte ordinacni hodiny pro kazdy den v tydnu</p>
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
          Novy slot
        </button>
      </div>

      {/* Create Form */}
      {showCreateForm && (
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 mb-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Vytvorit novy casovy slot</h2>
          <form onSubmit={handleCreate} className="space-y-4">
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-4">
              <div>
                <label htmlFor="create-day" className="block text-sm font-medium text-gray-700 mb-1">
                  Den
                </label>
                <select
                  id="create-day"
                  value={createDay}
                  onChange={(e) => setCreateDay(Number(e.target.value))}
                  className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-shadow"
                >
                  {ORDERED_DAYS.map((day) => (
                    <option key={day} value={day}>
                      {DAY_NAMES[day]}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label htmlFor="create-start" className="block text-sm font-medium text-gray-700 mb-1">
                  Zacatek
                </label>
                <input
                  id="create-start"
                  type="time"
                  value={createStart}
                  onChange={(e) => setCreateStart(e.target.value)}
                  className={`w-full px-4 py-2.5 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-shadow ${
                    createErrors.start ? 'border-red-300 bg-red-50' : 'border-gray-300'
                  }`}
                />
                {createErrors.start && <p className="mt-1 text-sm text-red-600">{createErrors.start}</p>}
              </div>
              <div>
                <label htmlFor="create-end" className="block text-sm font-medium text-gray-700 mb-1">
                  Konec
                </label>
                <input
                  id="create-end"
                  type="time"
                  value={createEnd}
                  onChange={(e) => setCreateEnd(e.target.value)}
                  className={`w-full px-4 py-2.5 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-shadow ${
                    createErrors.end ? 'border-red-300 bg-red-50' : 'border-gray-300'
                  }`}
                />
                {createErrors.end && <p className="mt-1 text-sm text-red-600">{createErrors.end}</p>}
              </div>
              <div className="flex items-end">
                <label className="flex items-center gap-2 cursor-pointer pb-2.5">
                  <input
                    type="checkbox"
                    checked={createAvailable}
                    onChange={(e) => setCreateAvailable(e.target.checked)}
                    className="w-4 h-4 text-teal-600 border-gray-300 rounded focus:ring-teal-500"
                  />
                  <span className="text-sm font-medium text-gray-700">Dostupny</span>
                </label>
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
          <p className="font-medium">Nepodarilo se nacist casove sloty</p>
          <p className="text-sm mt-1">{error}</p>
        </div>
      )}

      {/* Time Slots grouped by day */}
      {!loading && !error && timeSlots && (
        <div className="space-y-6">
          {ORDERED_DAYS.map((day) => {
            const slots = groupedSlots[day] || [];
            return (
              <div key={day} className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
                <div className="bg-gray-50 px-6 py-3 border-b border-gray-200">
                  <h3 className="text-sm font-semibold text-gray-700 uppercase tracking-wider">
                    {DAY_NAMES[day]}
                  </h3>
                </div>
                <div className="p-6">
                  {slots.length === 0 ? (
                    <p className="text-gray-400 text-sm">Zadne sloty pro tento den</p>
                  ) : (
                    <div className="space-y-3">
                      {slots.map((slot) => (
                        <div key={slot.id}>
                          {editingId === slot.id ? (
                            <form
                              onSubmit={handleUpdate}
                              className="bg-teal-50 rounded-lg p-4 border border-teal-200"
                            >
                              <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-3">
                                <div>
                                  <label className="block text-xs font-medium text-gray-700 mb-1">Den</label>
                                  <select
                                    value={editDay}
                                    onChange={(e) => setEditDay(Number(e.target.value))}
                                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent text-sm"
                                  >
                                    {ORDERED_DAYS.map((d) => (
                                      <option key={d} value={d}>
                                        {DAY_NAMES[d]}
                                      </option>
                                    ))}
                                  </select>
                                </div>
                                <div>
                                  <label className="block text-xs font-medium text-gray-700 mb-1">Zacatek</label>
                                  <input
                                    type="time"
                                    value={editStart}
                                    onChange={(e) => setEditStart(e.target.value)}
                                    className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent text-sm ${
                                      editErrors.start ? 'border-red-300 bg-red-50' : 'border-gray-300'
                                    }`}
                                  />
                                  {editErrors.start && (
                                    <p className="mt-1 text-xs text-red-600">{editErrors.start}</p>
                                  )}
                                </div>
                                <div>
                                  <label className="block text-xs font-medium text-gray-700 mb-1">Konec</label>
                                  <input
                                    type="time"
                                    value={editEnd}
                                    onChange={(e) => setEditEnd(e.target.value)}
                                    className={`w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent text-sm ${
                                      editErrors.end ? 'border-red-300 bg-red-50' : 'border-gray-300'
                                    }`}
                                  />
                                  {editErrors.end && (
                                    <p className="mt-1 text-xs text-red-600">{editErrors.end}</p>
                                  )}
                                </div>
                                <div className="flex items-end gap-2">
                                  <label className="flex items-center gap-2 cursor-pointer pb-2">
                                    <input
                                      type="checkbox"
                                      checked={editAvailable}
                                      onChange={(e) => setEditAvailable(e.target.checked)}
                                      className="w-4 h-4 text-teal-600 border-gray-300 rounded focus:ring-teal-500"
                                    />
                                    <span className="text-sm text-gray-700">Dostupny</span>
                                  </label>
                                </div>
                              </div>
                              <div className="flex justify-end gap-2 mt-3">
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
                          ) : (
                            <div className="flex items-center justify-between bg-gray-50 rounded-lg px-4 py-3 border border-gray-100">
                              <div className="flex items-center gap-4">
                                <span className="text-sm font-medium text-gray-900">
                                  {slot.startTime} - {slot.endTime}
                                </span>
                                <span
                                  className={`inline-flex items-center px-2 py-0.5 text-xs font-medium rounded-full ${
                                    slot.isAvailable
                                      ? 'bg-green-100 text-green-800'
                                      : 'bg-gray-200 text-gray-500'
                                  }`}
                                >
                                  {slot.isAvailable ? 'Dostupny' : 'Nedostupny'}
                                </span>
                              </div>
                              <div className="flex gap-2">
                                <button
                                  onClick={() => startEdit(slot)}
                                  className="px-3 py-1.5 text-xs font-medium text-blue-600 bg-blue-50 border border-blue-200 rounded-lg hover:bg-blue-100 transition-colors"
                                >
                                  Upravit
                                </button>
                                <button
                                  onClick={() => setDeleteConfirmId(slot.id)}
                                  className="px-3 py-1.5 text-xs font-medium text-red-600 bg-red-50 border border-red-200 rounded-lg hover:bg-red-100 transition-colors"
                                >
                                  Smazat
                                </button>
                              </div>
                            </div>
                          )}
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {deleteConfirmId && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
          <div
            className="absolute inset-0 bg-black/50"
            onClick={() => setDeleteConfirmId(null)}
          />
          <div className="relative bg-white rounded-xl shadow-xl p-6 max-w-sm w-full">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Smazat casovy slot</h3>
            <p className="text-gray-600 text-sm mb-6">
              Opravdu chcete smazat tento casovy slot? Tuto akci nelze vzit zpet.
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
                disabled={deletingSlot}
                className="px-4 py-2 text-sm font-medium text-white bg-red-600 rounded-lg hover:bg-red-700 disabled:opacity-50 transition-colors"
              >
                {deletingSlot ? 'Mazani...' : 'Smazat'}
              </button>
            </div>
          </div>
        </div>
      )}
    </Layout>
  );
}
