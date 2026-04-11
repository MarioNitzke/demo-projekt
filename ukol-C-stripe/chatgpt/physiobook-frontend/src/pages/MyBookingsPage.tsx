import React, { useState } from 'react';
import { useToast } from '@/contexts/ToastContext';
import { useBookings, useCancelBooking, usePayBooking } from '@/features/bookings/hooks';
import { BookingStatus, PaymentStatus } from '@/features/bookings/types';
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

export default function MyBookingsPage() {
  const [page, setPage] = useState(1);
  const pageSize = 10;
  const { data, loading, error, refetch } = useBookings(page, pageSize);
  const { cancelBooking, loading: cancelling } = useCancelBooking();
  const { showToast } = useToast();
  const [cancelConfirmId, setCancelConfirmId] = useState<string | null>(null);
  const { payBooking, loading: paying } = usePayBooking();

  async function handlePay(bookingId: string) {
    const { url } = await payBooking(bookingId);
    window.location.href = url;
  }

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleDateString('cs-CZ', {
      weekday: 'short',
      year: 'numeric',
      month: 'long',
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

  return (
    <Layout>
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">Moje rezervace</h1>
        <p className="text-gray-500 mt-1">Prehled vsech vasich rezervaci</p>
      </div>

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

      {!loading && !error && data && (
        <>
          {data.items.length === 0 ? (
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
              <p className="text-gray-500 text-lg">Zatim nemate zadne rezervace</p>
              <p className="text-gray-400 text-sm mt-1">Objednejte se na nasi stranku sluzeb</p>
            </div>
          ) : (
            <div className="space-y-4">
              {data.items.map((bookingItem) => {
                const status = statusConfig[bookingItem.status as BookingStatus] || statusConfig[BookingStatus.Pending];
                return (
                  <div
                    key={bookingItem.id}
                    className="bg-white rounded-xl shadow-sm border border-gray-200 p-5 hover:shadow-md transition-shadow"
                  >
                    <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                      <div className="flex-1">
                        <div className="flex items-center gap-3 mb-2">
                          <h3 className="text-lg font-semibold text-gray-900">
                            {bookingItem.serviceName || 'Sluzba'}
                          </h3>
                          <span
                            className={`inline-flex items-center px-2.5 py-0.5 text-xs font-medium rounded-full border ${status.className}`}
                          >
                            {status.label}
                          </span>
                        </div>
                        <div className="flex flex-wrap items-center gap-4 text-sm text-gray-500">
                          <span className="inline-flex items-center gap-1">
                            <svg
                              xmlns="http://www.w3.org/2000/svg"
                              className="h-4 w-4"
                              viewBox="0 0 20 20"
                              fill="currentColor"
                            >
                              <path
                                fillRule="evenodd"
                                d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z"
                                clipRule="evenodd"
                              />
                            </svg>
                            {formatDate(bookingItem.startTime)}
                          </span>
                          <span className="inline-flex items-center gap-1">
                            <svg
                              xmlns="http://www.w3.org/2000/svg"
                              className="h-4 w-4"
                              viewBox="0 0 20 20"
                              fill="currentColor"
                            >
                              <path
                                fillRule="evenodd"
                                d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z"
                                clipRule="evenodd"
                              />
                            </svg>
                            {formatTime(bookingItem.startTime)} - {formatTime(bookingItem.endTime)}
                          </span>
                        </div>
                        {bookingItem.notes && (
                          <p className="text-sm text-gray-400 mt-2 italic">
                            {bookingItem.notes}
                          </p>
                        )}
                      </div>
                      {bookingItem.paymentStatus === PaymentStatus.Unpaid && (
                        <button onClick={() => handlePay(bookingItem.id)}>Zaplatit</button>
                      )}
                      {canCancel(bookingItem.status as BookingStatus) && (
                        <button
                          onClick={() => setCancelConfirmId(bookingItem.id)}
                          className="px-4 py-2 text-sm font-medium text-red-600 bg-red-50 border border-red-200 rounded-lg hover:bg-red-100 transition-colors flex-shrink-0"
                        >
                          Zrusit
                        </button>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>
          )}

          {/* Pagination */}
          {data.totalPages > 1 && (
            <div className="flex justify-center items-center gap-2 mt-10">
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
              Opravdu chcete zrusit tuto rezervaci? Tuto akci nelze vzit zpet.
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
