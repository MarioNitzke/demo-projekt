import { useState, useEffect, useCallback } from 'react';
import { serviceService } from './serviceService';
import type { Service, ServicesPagedResponse, CreateServiceRequest, UpdateServiceRequest } from './types';

export function useServices(page: number = 1, pageSize: number = 10) {
  const [data, setData] = useState<ServicesPagedResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchServices = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await serviceService.getServices(page, pageSize);
      setData(response);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to fetch services';
      setError(message);
    } finally {
      setLoading(false);
    }
  }, [page, pageSize]);

  useEffect(() => {
    fetchServices();
  }, [fetchServices]);

  return { data, loading, error, refetch: fetchServices };
}

export function useService(id: string | undefined) {
  const [service, setService] = useState<Service | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) {
      setLoading(false);
      return;
    }

    let cancelled = false;
    const fetchService = async () => {
      setLoading(true);
      setError(null);
      try {
        const response = await serviceService.getServiceById(id);
        if (!cancelled) {
          setService(response);
        }
      } catch (err: unknown) {
        if (!cancelled) {
          const message = err instanceof Error ? err.message : 'Failed to fetch service';
          setError(message);
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    };

    fetchService();
    return () => {
      cancelled = true;
    };
  }, [id]);

  return { service, loading, error };
}

export function useCreateService() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const createService = useCallback(async (data: CreateServiceRequest): Promise<Service | null> => {
    setLoading(true);
    setError(null);
    try {
      const service = await serviceService.createService(data);
      return service;
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to create service';
      setError(message);
      return null;
    } finally {
      setLoading(false);
    }
  }, []);

  return { createService, loading, error };
}

export function useUpdateService() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const updateService = useCallback(
    async (id: string, data: UpdateServiceRequest): Promise<Service | null> => {
      setLoading(true);
      setError(null);
      try {
        const service = await serviceService.updateService(id, data);
        return service;
      } catch (err: unknown) {
        const message = err instanceof Error ? err.message : 'Failed to update service';
        setError(message);
        return null;
      } finally {
        setLoading(false);
      }
    },
    [],
  );

  return { updateService, loading, error };
}

export function useDeleteService() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const deleteService = useCallback(async (id: string): Promise<boolean> => {
    setLoading(true);
    setError(null);
    try {
      await serviceService.deleteService(id);
      return true;
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to delete service';
      setError(message);
      return false;
    } finally {
      setLoading(false);
    }
  }, []);

  return { deleteService, loading, error };
}
