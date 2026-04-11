import type { PagedResponse } from '@/shared/ApiResponse';

export interface Service {
  id: string;
  name: string;
  description: string;
  durationMinutes: number;
  price: number;
  isActive: boolean;
}

export interface CreateServiceRequest {
  name: string;
  description: string;
  durationMinutes: number;
  price: number;
}

export interface UpdateServiceRequest {
  name: string;
  description: string;
  durationMinutes: number;
  price: number;
}

export type ServicesPagedResponse = PagedResponse<Service>;
