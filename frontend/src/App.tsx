import { useState, useEffect, useCallback } from 'react';
import type {
  Employee,
  CreateEmployeeDto,
  UpdateEmployeeDto,
  EmployeeQueryParams,
  DashboardSummary,
  ValidationProblemDetails,
} from './types/employee';
import { employeeService } from './services/employees';
import { ApiError } from './services/api';
import { Header } from './components/Header';
import { SummaryCards } from './components/SummaryCards';
import { SearchInput } from './components/SearchInput';
import { EmployeeTable } from './components/EmployeeTable';
import { Pagination } from './components/Pagination';
import { EmployeeFormModal } from './components/EmployeeFormModal';
import { EmployeeDetailsModal } from './components/EmployeeDetailsModal';
import { ConfirmModal } from './components/ConfirmModal';
import { Toast, type ToastMessage } from './components/Toast';
import { RotateCcw } from 'lucide-react';

export function App() {
  // Data State
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [summary, setSummary] = useState<DashboardSummary | null>(null);
  const [departments, setDepartments] = useState<string[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(1);

  // Query / Filter State
  const [search, setSearch] = useState('');
  const [selectedDepartment, setSelectedDepartment] = useState('');
  const [activeFilter, setActiveFilter] = useState<'all' | 'true' | 'false'>('all');
  const [sortBy, setSortBy] = useState('name');
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('asc');
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  // UI / Modal State
  const [loading, setLoading] = useState(true);
  const [summaryLoading, setSummaryLoading] = useState(true);
  const [formLoading, setFormLoading] = useState(false);
  const [actionLoading, setActionLoading] = useState(false);

  const [formModalOpen, setFormModalOpen] = useState(false);
  const [editingEmployee, setEditingEmployee] = useState<Employee | null>(null);
  const [serverErrors, setServerErrors] = useState<ValidationProblemDetails | null>(null);

  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [selectedEmployee, setSelectedEmployee] = useState<Employee | null>(null);

  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false);
  const [employeeToDelete, setEmployeeToDelete] = useState<Employee | null>(null);

  const [toggleConfirmOpen, setToggleConfirmOpen] = useState(false);
  const [employeeToToggle, setEmployeeToToggle] = useState<Employee | null>(null);

  const [toast, setToast] = useState<ToastMessage | null>(null);

  const showToast = (type: 'success' | 'error', text: string) => {
    setToast({ id: String(Date.now()), type, text });
  };

  // Fetch summary and departments
  const loadMetadata = useCallback(async () => {
    try {
      setSummaryLoading(true);
      const [sumData, deptData] = await Promise.all([
        employeeService.getDashboardSummary(),
        employeeService.getDepartments(),
      ]);
      setSummary(sumData);
      setDepartments(deptData);
    } catch (err) {
      console.error('Failed to load metadata', err);
    } finally {
      setSummaryLoading(false);
    }
  }, []);

  // Fetch employee list with current filters
  const loadEmployees = useCallback(async () => {
    try {
      setLoading(true);
      const params: EmployeeQueryParams = {
        search: search.trim() || undefined,
        department: selectedDepartment || undefined,
        active: activeFilter === 'all' ? undefined : activeFilter === 'true',
        sortBy,
        sortOrder,
        page,
        pageSize,
      };

      const result = await employeeService.getEmployees(params);
      setEmployees(result.items);
      setTotalCount(result.totalCount);
      setTotalPages(result.totalPages);
    } catch (err) {
      console.error('Failed to load employees', err);
      showToast('error', 'Unable to fetch employees from backend.');
    } finally {
      setLoading(false);
    }
  }, [search, selectedDepartment, activeFilter, sortBy, sortOrder, page, pageSize]);

  useEffect(() => {
    loadMetadata();
  }, [loadMetadata]);

  useEffect(() => {
    loadEmployees();
  }, [loadEmployees]);

  // Sorting Handler
  const handleSort = (column: string) => {
    if (sortBy.toLowerCase() === column.toLowerCase()) {
      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(column);
      setSortOrder('asc');
    }
    setPage(1);
  };

  // Reset Filters
  const handleResetFilters = () => {
    setSearch('');
    setSelectedDepartment('');
    setActiveFilter('all');
    setSortBy('name');
    setSortOrder('asc');
    setPage(1);
  };

  // Create / Edit Action
  const handleOpenCreate = () => {
    setEditingEmployee(null);
    setServerErrors(null);
    setFormModalOpen(true);
  };

  const handleOpenEdit = (emp: Employee) => {
    setEditingEmployee(emp);
    setServerErrors(null);
    setFormModalOpen(true);
  };

  const handleFormSubmit = async (data: CreateEmployeeDto | UpdateEmployeeDto) => {
    try {
      setFormLoading(true);
      setServerErrors(null);

      if (editingEmployee) {
        await employeeService.updateEmployee(editingEmployee.id, data as UpdateEmployeeDto);
        showToast('success', `Employee ${data.firstName} ${data.lastName} updated successfully.`);
      } else {
        await employeeService.createEmployee(data as CreateEmployeeDto);
        showToast('success', `Employee ${data.firstName} ${data.lastName} created successfully.`);
      }

      setFormModalOpen(false);
      setEditingEmployee(null);
      await Promise.all([loadEmployees(), loadMetadata()]);
    } catch (err) {
      if (err instanceof ApiError && err.problemDetails) {
        setServerErrors(err.problemDetails);
      } else {
        showToast('error', err instanceof Error ? err.message : 'An error occurred while saving.');
      }
    } finally {
      setFormLoading(false);
    }
  };

  // View Details
  const handleViewDetails = (emp: Employee) => {
    setSelectedEmployee(emp);
    setDetailsModalOpen(true);
  };

  // Toggle Status
  const handleOpenToggle = (emp: Employee) => {
    setEmployeeToToggle(emp);
    setToggleConfirmOpen(true);
  };

  const handleConfirmToggle = async () => {
    if (!employeeToToggle) return;
    try {
      setActionLoading(true);
      await employeeService.toggleEmployeeStatus(employeeToToggle.id);
      showToast(
        'success',
        `Employee status changed to ${employeeToToggle.active ? 'Inactive' : 'Active'}.`
      );
      setToggleConfirmOpen(false);
      setEmployeeToToggle(null);
      await Promise.all([loadEmployees(), loadMetadata()]);
    } catch (err) {
      showToast('error', err instanceof Error ? err.message : 'Failed to update status.');
    } finally {
      setActionLoading(false);
    }
  };

  // Delete
  const handleOpenDelete = (emp: Employee) => {
    setEmployeeToDelete(emp);
    setDeleteConfirmOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!employeeToDelete) return;
    try {
      setActionLoading(true);
      await employeeService.deleteEmployee(employeeToDelete.id);
      showToast('success', `Employee ${employeeToDelete.fullName} deleted successfully.`);
      setDeleteConfirmOpen(false);
      setEmployeeToDelete(null);
      await Promise.all([loadEmployees(), loadMetadata()]);
    } catch (err) {
      showToast('error', err instanceof Error ? err.message : 'Failed to delete employee.');
    } finally {
      setActionLoading(false);
    }
  };

  const isFiltered = Boolean(search || selectedDepartment || activeFilter !== 'all');

  return (
    <div className="app-container">
      <Header onAddClick={handleOpenCreate} />

      <main className="main-content">
        {/* KPI / Summary Cards */}
        <SummaryCards summary={summary} loading={summaryLoading} />

        {/* Toolbar: Search, Filters & Actions */}
        <div className="toolbar-card">
          <div className="toolbar-filters">
            <SearchInput
              value={search}
              onChange={(val) => {
                setSearch(val);
                setPage(1);
              }}
            />

            <select
              className="filter-select"
              value={selectedDepartment}
              onChange={(e) => {
                setSelectedDepartment(e.target.value);
                setPage(1);
              }}
            >
              <option value="">All Departments</option>
              {departments.map((dept) => (
                <option key={dept} value={dept}>
                  {dept}
                </option>
              ))}
            </select>

            <select
              className="filter-select"
              value={activeFilter}
              onChange={(e) => {
                setActiveFilter(e.target.value as 'all' | 'true' | 'false');
                setPage(1);
              }}
            >
              <option value="all">All Statuses</option>
              <option value="true">Active Only</option>
              <option value="false">Inactive Only</option>
            </select>

            {isFiltered && (
              <button
                type="button"
                className="btn btn--outline-subtle"
                onClick={handleResetFilters}
                title="Reset all filters"
              >
                <RotateCcw size={14} /> Clear
              </button>
            )}
          </div>
        </div>

        {/* Employee Table & Pagination */}
        <div className="table-card">
          <EmployeeTable
            employees={employees}
            loading={loading}
            sortBy={sortBy}
            sortOrder={sortOrder}
            onSort={handleSort}
            onView={handleViewDetails}
            onEdit={handleOpenEdit}
            onToggleStatus={handleOpenToggle}
            onDelete={handleOpenDelete}
          />

          <Pagination
            currentPage={page}
            totalPages={totalPages}
            totalCount={totalCount}
            pageSize={pageSize}
            onPageChange={(p) => setPage(p)}
            onPageSizeChange={(s) => {
              setPageSize(s);
              setPage(1);
            }}
          />
        </div>
      </main>

      {/* Modals */}
      <EmployeeFormModal
        isOpen={formModalOpen}
        employee={editingEmployee}
        departments={departments}
        onClose={() => setFormModalOpen(false)}
        onSubmit={handleFormSubmit}
        loading={formLoading}
        serverErrors={serverErrors}
      />

      <EmployeeDetailsModal
        isOpen={detailsModalOpen}
        employee={selectedEmployee}
        onClose={() => setDetailsModalOpen(false)}
        onEdit={(emp) => {
          setDetailsModalOpen(false);
          handleOpenEdit(emp);
        }}
      />

      <ConfirmModal
        isOpen={toggleConfirmOpen}
        title={employeeToToggle?.active ? 'Deactivate Employee' : 'Activate Employee'}
        message={
          employeeToToggle?.active
            ? `Are you sure you want to deactivate ${employeeToToggle?.fullName}? They will be marked as inactive in company directory.`
            : `Are you sure you want to activate ${employeeToToggle?.fullName}?`
        }
        confirmLabel={employeeToToggle?.active ? 'Deactivate' : 'Activate'}
        onConfirm={handleConfirmToggle}
        onCancel={() => setToggleConfirmOpen(false)}
        loading={actionLoading}
      />

      <ConfirmModal
        isOpen={deleteConfirmOpen}
        title="Delete Employee"
        message={`Are you sure you want to permanently delete ${employeeToDelete?.fullName}? This action cannot be undone.`}
        confirmLabel="Delete"
        isDestructive
        onConfirm={handleConfirmDelete}
        onCancel={() => setDeleteConfirmOpen(false)}
        loading={actionLoading}
      />

      <Toast toast={toast} onDismiss={() => setToast(null)} />
    </div>
  );
}

export default App;
