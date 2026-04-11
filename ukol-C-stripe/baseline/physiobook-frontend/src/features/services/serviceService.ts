import api from '@/shared/BaseApiService';
import type { Service, CreateServiceRequest, UpdateServiceRequest, ServicesPagedResponse } from './types';

export const serviceService = {
  async getServices(page: number = 1, pageSize: number = 10): Promise<ServicesPagedResponse> {
    const response = await api.get<ServicesPagedResponse>('/services', {
      params: { pageNumber: page, pageSize },
    });
    return response.data;
  },

  async getServiceById(id: string): Promise<Service> {
    const response = await api.get<Service>(`/services/${id}`);
    return response.data;
  },

  async createService(data: CreateServiceRequest): Promise<Service> {
    const response = await api.post<Service>('/services', data);
    return response.data;
  },

  async updateService(id: string, data: UpdateServiceRequest): Promise<Service> {
    const response = await api.put<Service>(`/services/${id}`, data);
    return response.data;
  },

  async deleteService(id: string): Promise<void> {
    await api.delete(`/services/${id}`);
  },
};
