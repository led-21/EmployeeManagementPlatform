import { api } from './api';
import type {
  Employee,
  CreateEmployeeDto,
  UpdateEmployeeDto,
  EmployeeQueryParams,
  PagedResult,
  DashboardSummary,
} from '../types/employee';

export const employeeService = {
  async getEmployees(params: EmployeeQueryParams = {}): Promise<PagedResult<Employee>> {
    const searchParams = new URLSearchParams();

    if (params.search) searchParams.append('search', params.search);
    if (params.department) searchParams.append('department', params.department);
    if (params.active !== undefined) searchParams.append('active', String(params.active));
    if (params.sortBy) searchParams.append('sortBy', params.sortBy);
    if (params.sortOrder) searchParams.append('sortOrder', params.sortOrder);
    if (params.page) searchParams.append('page', String(params.page));
    if (params.pageSize) searchParams.append('pageSize', String(params.pageSize));

    const queryString = searchParams.toString();
    const endpoint = queryString ? `/employees?${queryString}` : '/employees';
    return api.get<PagedResult<Employee>>(endpoint);
  },

  async getEmployeeById(id: number): Promise<Employee> {
    return api.get<Employee>(`/employees/${id}`);
  },

  async createEmployee(dto: CreateEmployeeDto): Promise<Employee> {
    return api.post<Employee>('/employees', dto);
  },

  async updateEmployee(id: number, dto: UpdateEmployeeDto): Promise<Employee> {
    return api.put<Employee>(`/employees/${id}`, dto);
  },

  async toggleEmployeeStatus(id: number): Promise<void> {
    await api.patch(`/employees/${id}/status`);
  },

  async deleteEmployee(id: number): Promise<void> {
    await api.delete(`/employees/${id}`);
  },

  async getDashboardSummary(): Promise<DashboardSummary> {
    return api.get<DashboardSummary>('/employees/summary');
  },

  async getDepartments(): Promise<string[]> {
    return api.get<string[]>('/employees/departments');
  },
};
