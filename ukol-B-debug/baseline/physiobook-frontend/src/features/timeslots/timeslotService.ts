import api from '@/shared/BaseApiService';
import type { TimeSlot, CreateTimeSlotRequest, UpdateTimeSlotRequest } from './types';

export const timeslotService = {
  async getTimeSlots(dayOfWeek?: number): Promise<TimeSlot[]> {
    const response = await api.get<{ items: TimeSlot[] }>('/timeslots', {
      params: dayOfWeek !== undefined ? { dayOfWeek } : undefined,
    });
    return response.data.items;
  },

  async createTimeSlot(data: CreateTimeSlotRequest): Promise<TimeSlot> {
    const response = await api.post<TimeSlot>('/timeslots', data);
    return response.data;
  },

  async updateTimeSlot(id: string, data: UpdateTimeSlotRequest): Promise<TimeSlot> {
    const response = await api.put<TimeSlot>(`/timeslots/${id}`, data);
    return response.data;
  },

  async deleteTimeSlot(id: string): Promise<void> {
    await api.delete(`/timeslots/${id}`);
  },
};
