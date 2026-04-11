import { useState, useEffect, useCallback } from 'react';
import { timeslotService } from './timeslotService';
import type { TimeSlot, CreateTimeSlotRequest, UpdateTimeSlotRequest } from './types';

export function useTimeSlots(dayOfWeek?: number) {
  const [data, setData] = useState<TimeSlot[] | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchTimeSlots = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await timeslotService.getTimeSlots(dayOfWeek);
      setData(response);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to fetch timeslots';
      setError(message);
    } finally {
      setLoading(false);
    }
  }, [dayOfWeek]);

  useEffect(() => {
    fetchTimeSlots();
  }, [fetchTimeSlots]);

  return { data, loading, error, refetch: fetchTimeSlots };
}

export function useCreateTimeSlot() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const createTimeSlot = useCallback(async (data: CreateTimeSlotRequest): Promise<TimeSlot | null> => {
    setLoading(true);
    setError(null);
    try {
      const timeSlot = await timeslotService.createTimeSlot(data);
      return timeSlot;
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to create timeslot';
      setError(message);
      return null;
    } finally {
      setLoading(false);
    }
  }, []);

  return { createTimeSlot, loading, error };
}

export function useUpdateTimeSlot() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const updateTimeSlot = useCallback(
    async (id: string, data: UpdateTimeSlotRequest): Promise<TimeSlot | null> => {
      setLoading(true);
      setError(null);
      try {
        const timeSlot = await timeslotService.updateTimeSlot(id, data);
        return timeSlot;
      } catch (err: unknown) {
        const message = err instanceof Error ? err.message : 'Failed to update timeslot';
        setError(message);
        return null;
      } finally {
        setLoading(false);
      }
    },
    [],
  );

  return { updateTimeSlot, loading, error };
}

export function useDeleteTimeSlot() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const deleteTimeSlot = useCallback(async (id: string): Promise<boolean> => {
    setLoading(true);
    setError(null);
    try {
      await timeslotService.deleteTimeSlot(id);
      return true;
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to delete timeslot';
      setError(message);
      return false;
    } finally {
      setLoading(false);
    }
  }, []);

  return { deleteTimeSlot, loading, error };
}
