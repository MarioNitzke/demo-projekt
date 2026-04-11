import type { PagedResponse } from '@/shared/ApiResponse';

export enum BookingStatus {
  Pending = 'Pending',
  Confirmed = 'Confirmed',
  Cancelled = 'Cancelled',
  Completed = 'Completed',
}

export enum PaymentStatus {
  Unpaid = 'Unpaid',
  Paid = 'Paid',
  Expired = 'Expired',
}

export interface Booking {
  id: string;
  clientId: string;
  serviceId: string;
  serviceName?: string;
  startTime: string;
  endTime: string;
  status: BookingStatus;
  paymentStatus: PaymentStatus;
  notes?: string;
  createdAt: string;
  updatedAt?: string;
  cancelledAt?: string;
}

export interface CreateCheckoutSessionResponse {
  sessionUrl: string;
}

export interface AvailableSlot {
  startTime: string; // "HH:mm"
  endTime: string;   // "HH:mm"
}

export interface AvailabilityResponse {
  date: string;
  serviceId: string;
  serviceName: string;
  availableSlots: AvailableSlot[];
}

export interface CreateBookingRequest {
  serviceId: string;
  startTime: string; // ISO datetime
  notes?: string;
}

export type BookingsPagedResponse = PagedResponse<Booking>;
