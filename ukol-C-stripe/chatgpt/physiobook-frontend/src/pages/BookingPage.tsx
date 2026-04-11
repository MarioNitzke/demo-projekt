import React, { useState, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useToast } from '@/contexts/ToastContext';
import { useService } from '@/features/services/hooks';
import { useAvailability, useCreateBooking } from '@/features/bookings/hooks';
import Layout from '@/components/Layout';

export default function BookingPage() {
  const { serviceId } = useParams<{ serviceId: string }>();
  const navigate = useNavigate();
  const { showToast } = useToast();

  const { service, loading: serviceLoading, error: serviceError } = useService(serviceId);
  const [selectedDate, setSelectedDate] = useState<string>('');
  const [selectedSlot, setSelectedSlot] = useState<{ startTime: string; endTime: string } | null>(null);
  const [notes, setNotes] = useState('');

  const { data: availability, loading: slotsLoading, error: slotsError } = useAvailability(
    serviceId,
    selectedDate || undefined,
  );
  const { createBooking, loading: booking } = useCreateBooking();

  const tomorrow = useMemo(() => {
    const d = new Date();
    d.setDate(d.getDate() + 1);
    return d.toISOString().split('T')[0];
  }, []);

  const handleDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSelectedDate(e.target.value);
    setSelectedSlot(null);
  };

  const handleSlotClick = (slot: { startTime: string; endTime: string }) => {
    setSelectedSlot(slot);
  };

  const handleSubmit = async () => {
    if (!serviceId || !selectedDate || !selectedSlot) return;

    const startTimeISO = `${selectedDate}T${selectedSlot.startTime}:00Z`;

    const result = await createBooking({
      serviceId,
      startTime: startTimeISO,
      notes: notes.trim() || undefined,
    });

    if (result) {
      showToast('Rezervace byla uspesne vytvorena!', 'success');
      navigate('/my-bookings');
    } else {
      showToast('Nepodarilo se vytvorit rezervaci. Zkuste to prosim znovu.', 'error');
    }
  };

  if (serviceLoading) {
    return (
      <Layout>
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
      </Layout>
    );
  }

  if (serviceError || !service) {
    return (
      <Layout>
        <div className="text-center py-20">
          <div className="p-6 bg-red-50 border border-red-200 rounded-xl text-red-700 inline-block">
            <p className="font-medium">Sluzba nenalezena</p>
            <p className="text-sm mt-1">{serviceError || 'Pozadovana sluzba neexistuje.'}</p>
          </div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-3xl mx-auto">
        {/* Service Info */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 mb-6">
          <div className="flex items-start gap-4">
            <div className="w-12 h-12 bg-teal-100 rounded-lg flex items-center justify-center flex-shrink-0">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                className="h-6 w-6 text-teal-600"
                viewBox="0 0 20 20"
                fill="currentColor"
              >
                <path
                  fillRule="evenodd"
                  d="M3.172 5.172a4 4 0 015.656 0L10 6.343l1.172-1.171a4 4 0 115.656 5.656L10 17.657l-6.828-6.829a4 4 0 010-5.656z"
                  clipRule="evenodd"
                />
              </svg>
            </div>
            <div>
              <h1 className="text-2xl font-bold text-gray-900">{service.name}</h1>
              <p className="text-gray-600 mt-1">{service.description}</p>
              <div className="flex items-center gap-4 mt-3 text-sm">
                <span className="inline-flex items-center gap-1 text-gray-500">
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
                  {service.durationMinutes} min
                </span>
                <span className="font-semibold text-teal-700">{service.price} Kc</span>
              </div>
            </div>
          </div>
        </div>

        {/* Date Picker */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 mb-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">1. Vyberte datum</h2>
          <input
            type="date"
            min={tomorrow}
            value={selectedDate}
            onChange={handleDateChange}
            className="w-full sm:w-auto px-4 py-2.5 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-shadow"
          />
        </div>

        {/* Available Slots */}
        {selectedDate && (
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 mb-6">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">2. Vyberte cas</h2>

            {slotsLoading && (
              <div className="flex justify-center items-center py-10">
                <svg
                  className="animate-spin h-6 w-6 text-teal-600"
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

            {slotsError && (
              <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
                Nepodarilo se nacist volne terminy. Zkuste to prosim znovu.
              </div>
            )}

            {!slotsLoading && !slotsError && availability && (
              <>
                {availability.availableSlots.length === 0 ? (
                  <div className="text-center py-8">
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      className="h-12 w-12 text-gray-300 mx-auto mb-3"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={1.5}
                        d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                      />
                    </svg>
                    <p className="text-gray-500">Na vybrany den nejsou volne terminy</p>
                    <p className="text-gray-400 text-sm mt-1">Zkuste prosim jiny den</p>
                  </div>
                ) : (
                  <div className="grid grid-cols-3 sm:grid-cols-4 md:grid-cols-5 gap-3">
                    {availability.availableSlots.map((slot) => (
                      <button
                        key={`${slot.startTime}-${slot.endTime}`}
                        onClick={() => handleSlotClick(slot)}
                        className={`px-3 py-3 text-sm font-medium rounded-lg border transition-all ${
                          selectedSlot?.startTime === slot.startTime
                            ? 'bg-teal-600 text-white border-teal-600 shadow-md'
                            : 'bg-white text-gray-700 border-gray-300 hover:border-teal-400 hover:bg-teal-50'
                        }`}
                      >
                        {slot.startTime}
                      </button>
                    ))}
                  </div>
                )}
              </>
            )}
          </div>
        )}

        {/* Confirmation */}
        {selectedSlot && (
          <div className="bg-white rounded-xl shadow-sm border border-teal-200 p-6">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">3. Potvrzeni rezervace</h2>

            <div className="bg-teal-50 rounded-lg p-4 mb-4">
              <div className="grid grid-cols-2 gap-3 text-sm">
                <div>
                  <span className="text-gray-500">Sluzba:</span>
                  <p className="font-medium text-gray-900">{service.name}</p>
                </div>
                <div>
                  <span className="text-gray-500">Datum:</span>
                  <p className="font-medium text-gray-900">
                    {new Date(selectedDate).toLocaleDateString('cs-CZ', {
                      weekday: 'long',
                      year: 'numeric',
                      month: 'long',
                      day: 'numeric',
                    })}
                  </p>
                </div>
                <div>
                  <span className="text-gray-500">Cas:</span>
                  <p className="font-medium text-gray-900">
                    {selectedSlot.startTime} - {selectedSlot.endTime}
                  </p>
                </div>
                <div>
                  <span className="text-gray-500">Cena:</span>
                  <p className="font-medium text-teal-700">{service.price} Kc</p>
                </div>
              </div>
            </div>

            <div className="mb-4">
              <label htmlFor="notes" className="block text-sm font-medium text-gray-700 mb-1">
                Poznamky (nepovinne)
              </label>
              <textarea
                id="notes"
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
                rows={3}
                className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-shadow resize-y"
                placeholder="Napiste nam sve poznamky ci pozadavky..."
              />
            </div>

            <button
              onClick={handleSubmit}
              disabled={booking}
              className="w-full px-6 py-3 text-sm font-medium text-white bg-teal-600 rounded-lg hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              {booking ? (
                <span className="inline-flex items-center justify-center gap-2">
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
                  Vytvarim rezervaci...
                </span>
              ) : (
                'Potvrdit rezervaci'
              )}
            </button>
          </div>
        )}
      </div>
    </Layout>
  );
}
