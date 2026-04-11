import React, { useState } from 'react';
import { useToast } from '@/contexts/ToastContext';
import { useBookings, useCancelBooking } from '@/features/bookings/hooks';
import { BookingStatus } from '@/features/bookings/types';
import Layout from '@/components/Layout';

const statusConfig: Record<BookingStatus, { label: string; className: string }> = {
  [BookingStatus.Pending]: {
    label: 'Cekajici',
    className: 'bg-yellow-100 text-yellow-800 border-yellow-200',
  },
  [BookingStatus.Confirmed]: {
    label: 'Potvrzena',
    className: 'bg-green-100 text-green-800 border-green-200',
  },
  [BookingStatus.Cancelled]: {
    label: 'Zrusena',
    className: 'bg-red-100 text-red-800 border-red-200',
  },
  [BookingStatus.Completed]: {
    label: 'Dokoncena',
    className: 'bg-gray-100 text-gray-800 border-gray-200',
  },
};

const STATUS_OPTIONS: { value: string; label: string }[] = [
  { value: 'all', label: 'Vse' },
  { value: BookingStatus.Pending, label: 'Cekajici' },
  { value: BookingStatus.Confirmed, label: 'Potvrzene' },
  { value: BookingStatus.Cancelled, label: 'Zrusene' },
  { value: BookingStatus.Completed, label: 'Dokoncene' },
];

export default function AdminBookingsPage() {
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const pageSize = 15;
  const { data, loading, error, refetch } = useBookings(page, pageSize);
  const { cancelBooking, loading: cancelling } = useCancelBooking();
  const { showToast } = useToast();
  const [cancelConfirmId, setCancelConfirmId] = useState<string | null>(null);

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleDateString('cs-CZ', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  };

  const formatTime = (dateStr: string) => {
    return new Date(dateStr).toLocaleTimeString('cs-CZ', {
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const handleCancel = async () => {
    if (!cancelConfirmId) return;
    const result = await cancelBooking(cancelConfirmId);
    if (result) {
      showToast('Rezervace byla uspesne zrusena.', 'success');
      refetch();
    } else {
      showToast('Nepodarilo se zrusit rezervaci.', 'error');
    }
    setCancelConfirmId(null);
  };

  const canCancel = (status: BookingStatus) =>
    status === BookingStatus.Pending || status === BookingStatus.Confirmed;

  const filteredBookings =
    data?.items.filter((b) => statusFilter === 'all' || b.status === statusFilter) ?? [];

  return (
    <Layout>
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Sprava rezervaci</h1>
          <p className="text-gray-500 mt-1">Prehled vsech rezervaci v systemu</p>
        </div>
      </div>

      {/* Status Filter */}
      <div className="mb-6 flex flex-wrap gap-2">
        {STATUS_OPTIONS.map((option) => (
          <button
            key={option.value}
            onClick={() => {
              setStatusFilter(option.value);
              setPage(1);
            }}
            className={`px-4 py-2 text-sm font-medium rounded-lg transition-colors ${
              statusFilter === option.value
                ? 'bg-teal-600 text-white'
                : 'bg-white text-gray-700 border border-gray-300 hover:bg-gray-50'
            }`}
          >
            {option.label}
          </button>
        ))}
      </div>

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
          <p className="font-medium">Nepodarilo se nacist rezervace</p>
          <p className="text-sm mt-1">{error}</p>
        </div>
      )}

      {/* Bookings Table */}
      {!loading && !error && data && (
        <>
          {filteredBookings.length === 0 ? (
            <div className="text-center py-20">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                className="h-16 w-16 text-gray-300 mx-auto mb-4"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={1}
                  d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
                />
              </svg>
              <p className="text-gray-500 text-lg">Zadne rezervace</p>
              <p className="text-gray-400 text-sm mt-1">
                {statusFilter !== 'all'
                  ? 'Pro zvoleny filtr nebyly nalezeny zadne rezervace.'
                  : 'V systemu zatim nejsou zadne rezervace.'}
              </p>
            </div>
          ) : (
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead>
                    <tr className="bg-gray-50 border-b border-gray-200">
                      <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">
                        Klient
                      </th>
                      <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">
                        Sluzba
                      </th>
                      <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">
                        Datum
                      </th>
                      <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">
                        Cas
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
                    {filteredBookings.map((bookingItem) => {
                      const status =
                        statusConfig[bookingItem.status] || statusConfig[BookingStatus.Pending];
                      return (
                        <tr key={bookingItem.id} className="hover:bg-gray-50 transition-colors">
                          <td className="px-6 py-4 text-sm text-gray-900">
                            {bookingItem.clientId}
                          </td>
                          <td className="px-6 py-4 text-sm text-gray-900 font-medium">
                            {bookingItem.serviceName || '-'}
                          </td>
                          <td className="px-6 py-4 text-sm text-gray-600">
                            {formatDate(bookingItem.startTime)}
                          </td>
                          <td className="px-6 py-4 text-sm text-gray-600">
                            {formatTime(bookingItem.startTime)} - {formatTime(bookingItem.endTime)}
                          </td>
                          <td className="px-6 py-4">
                            <span
                              className={`inline-flex items-center px-2.5 py-0.5 text-xs font-medium rounded-full border ${status.className}`}
                            >
                              {status.label}
                            </span>
                          </td>
                          <td className="px-6 py-4 text-right">
                            {canCancel(bookingItem.status) && (
                              <button
                                onClick={() => setCancelConfirmId(bookingItem.id)}
                                className="px-3 py-1.5 text-sm font-medium text-red-600 bg-red-50 border border-red-200 rounded-lg hover:bg-red-100 transition-colors"
                              >
                                Zrusit
                              </button>
                            )}
                          </td>
                        </tr>
                      );
                    })}
                  </tbody>
                </table>
              </div>
            </div>
          )}

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

      {/* Cancel Confirmation Modal */}
      {cancelConfirmId && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
          <div
            className="absolute inset-0 bg-black/50"
            onClick={() => setCancelConfirmId(null)}
          />
          <div className="relative bg-white rounded-xl shadow-xl p-6 max-w-sm w-full">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Zrusit rezervaci</h3>
            <p className="text-gray-600 text-sm mb-6">
              Opravdu chcete zrusit tuto rezervaci? Klient bude informovan.
            </p>
            <div className="flex justify-end gap-3">
              <button
                onClick={() => setCancelConfirmId(null)}
                className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
              >
                Zpet
              </button>
              <button
                onClick={handleCancel}
                disabled={cancelling}
                className="px-4 py-2 text-sm font-medium text-white bg-red-600 rounded-lg hover:bg-red-700 disabled:opacity-50 transition-colors"
              >
                {cancelling ? 'Rusin...' : 'Ano, zrusit'}
              </button>
            </div>
          </div>
        </div>
      )}
    </Layout>
  );
}
