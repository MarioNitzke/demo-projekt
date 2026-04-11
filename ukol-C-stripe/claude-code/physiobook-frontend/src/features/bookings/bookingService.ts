import api from '@/shared/BaseApiService';
import type { Booking, CreateBookingRequest, BookingsPagedResponse, AvailabilityResponse, CreateCheckoutSessionResponse } from './types';

export const bookingService = {
  async getBookings(page: number = 1, pageSize: number = 10): Promise<BookingsPagedResponse> {
    const response = await api.get<BookingsPagedResponse>('/bookings', {
      params: { pageNumber: page, pageSize },
    });
    return response.data;
  },

  async getBookingById(id: string): Promise<Booking> {
    const response = await api.get<Booking>(`/bookings/${id}`);
    return response.data;
  },

  async createBooking(data: CreateBookingRequest): Promise<Booking> {
    try {
      const response = await api.post<Booking>('/bookings', data);
      return response.data;
    } catch (error: any) {
      if (error.response?.status === 409) {
        throw new Error('Slot byl mezitím obsazen. Vyberte prosím jiný termín.');
      }
      throw error;
    }
  },

  async cancelBooking(id: string): Promise<Booking> {
    const response = await api.put<Booking>(`/bookings/${id}/cancel`);
    return response.data;
  },

  async getAvailability(serviceId: string, date: string): Promise<AvailabilityResponse> {
    const response = await api.get<AvailabilityResponse>('/bookings/availability', {
      params: { serviceId, date },
    });
    return response.data;
  },

  async createCheckoutSession(bookingId: string): Promise<CreateCheckoutSessionResponse> {
    const response = await api.post<CreateCheckoutSessionResponse>(
      `/bookings/${bookingId}/create-checkout-session`
    );
    return response.data;
  },
};
