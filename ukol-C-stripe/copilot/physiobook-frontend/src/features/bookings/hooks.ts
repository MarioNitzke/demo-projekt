import { useState, useEffect, useCallback } from 'react';
import { bookingService } from './bookingService';
import type {
  Booking,
  BookingsPagedResponse,
  CreateBookingRequest,
  AvailabilityResponse,
  CreateCheckoutSessionResponse,
} from './types';

export function useBookings(page: number = 1, pageSize: number = 10) {
  const [data, setData] = useState<BookingsPagedResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchBookings = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await bookingService.getBookings(page, pageSize);
      setData(response);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to fetch bookings';
      setError(message);
    } finally {
      setLoading(false);
    }
  }, [page, pageSize]);

  useEffect(() => {
    fetchBookings();
  }, [fetchBookings]);

  return { data, loading, error, refetch: fetchBookings };
}

export function useBooking(id: string | undefined) {
  const [booking, setBooking] = useState<Booking | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) {
      setLoading(false);
      return;
    }

    let cancelled = false;
    const fetchBooking = async () => {
      setLoading(true);
      setError(null);
      try {
        const response = await bookingService.getBookingById(id);
        if (!cancelled) {
          setBooking(response);
        }
      } catch (err: unknown) {
        if (!cancelled) {
          const message = err instanceof Error ? err.message : 'Failed to fetch booking';
          setError(message);
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    };

    fetchBooking();
    return () => {
      cancelled = true;
    };
  }, [id]);

  return { booking, loading, error };
}

export function useCreateBooking() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const createBooking = useCallback(async (data: CreateBookingRequest): Promise<Booking | null> => {
    setLoading(true);
    setError(null);
    try {
      const booking = await bookingService.createBooking(data);
      return booking;
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to create booking';
      setError(message);
      return null;
    } finally {
      setLoading(false);
    }
  }, []);

  return { createBooking, loading, error };
}

export function useCancelBooking() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const cancelBooking = useCallback(async (id: string): Promise<Booking | null> => {
    setLoading(true);
    setError(null);
    try {
      const booking = await bookingService.cancelBooking(id);
      return booking;
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to cancel booking';
      setError(message);
      return null;
    } finally {
      setLoading(false);
    }
  }, []);

  return { cancelBooking, loading, error };
}

export function useCreateCheckoutSession() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const createCheckoutSession = useCallback(async (bookingId: string): Promise<CreateCheckoutSessionResponse | null> => {
    setLoading(true);
    setError(null);
    try {
      const response = await bookingService.createCheckoutSession(bookingId);
      return response;
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to create checkout session';
      setError(message);
      return null;
    } finally {
      setLoading(false);
    }
  }, []);

  return { createCheckoutSession, loading, error };
}

export function useAvailability(serviceId: string | undefined, date: string | undefined) {
  const [data, setData] = useState<AvailabilityResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!serviceId || !date) {
      setLoading(false);
      return;
    }

    let cancelled = false;
    const fetchAvailability = async () => {
      setLoading(true);
      setError(null);
      try {
        const response = await bookingService.getAvailability(serviceId, date);
        if (!cancelled) {
          setData(response);
        }
      } catch (err: unknown) {
        if (!cancelled) {
          const message = err instanceof Error ? err.message : 'Failed to fetch availability';
          setError(message);
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    };

    fetchAvailability();
    return () => {
      cancelled = true;
    };
  }, [serviceId, date]);

  return { data, loading, error };
}
