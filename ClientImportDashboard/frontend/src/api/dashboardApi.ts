import { httpClient } from './httpClient';
import type { DashboardSummary } from './types';

export const dashboardApi = {
  getSummary: async (): Promise<DashboardSummary> => {
    const response = await httpClient.get<DashboardSummary>('/api/v1/dashboard');
    return response.data;
  },
};
