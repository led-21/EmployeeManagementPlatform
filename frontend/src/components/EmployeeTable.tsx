import React from 'react';
import { ArrowUpDown, ArrowUp, ArrowDown, Eye, Edit2, Trash2, Power } from 'lucide-react';
import type { Employee } from '../types/employee';
import { StatusBadge } from './StatusBadge';

interface EmployeeTableProps {
  employees: Employee[];
  loading: boolean;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  onSort: (column: string) => void;
  onView: (employee: Employee) => void;
  onEdit: (employee: Employee) => void;
  onToggleStatus: (employee: Employee) => void;
  onDelete: (employee: Employee) => void;
}

export const EmployeeTable: React.FC<EmployeeTableProps> = ({
  employees,
  loading,
  sortBy,
  sortOrder,
  onSort,
  onView,
  onEdit,
  onToggleStatus,
  onDelete,
}) => {
  const renderSortIcon = (column: string) => {
    if (sortBy?.toLowerCase() !== column.toLowerCase()) {
      return <ArrowUpDown size={14} className="sort-icon sort-icon--neutral" />;
    }
    return sortOrder === 'desc' ? (
      <ArrowDown size={14} className="sort-icon sort-icon--active" />
    ) : (
      <ArrowUp size={14} className="sort-icon sort-icon--active" />
    );
  };

  const formatCurrency = (val: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      maximumFractionDigits: 0,
    }).format(val);
  };

  const formatDate = (dateStr: string) => {
    try {
      const d = new Date(dateStr);
      return d.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
      });
    } catch {
      return dateStr;
    }
  };

  if (loading) {
    return (
      <div className="table-responsive">
        <table className="employee-table">
          <thead>
            <tr>
              <th>Employee</th>
              <th>Department</th>
              <th>Position</th>
              <th>Salary</th>
              <th>Hire Date</th>
              <th>Status</th>
              <th className="text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {[1, 2, 3, 4, 5].map((i) => (
              <tr key={i} className="table-row-skeleton">
                <td><div className="skeleton-line skeleton-line--md" /></td>
                <td><div className="skeleton-line skeleton-line--sm" /></td>
                <td><div className="skeleton-line skeleton-line--sm" /></td>
                <td><div className="skeleton-line skeleton-line--xs" /></td>
                <td><div className="skeleton-line skeleton-line--xs" /></td>
                <td><div className="skeleton-line skeleton-line--xs" /></td>
                <td><div className="skeleton-line skeleton-line--xs" /></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    );
  }

  if (employees.length === 0) {
    return (
      <div className="empty-state">
        <div className="empty-state__icon">👥</div>
        <h4 className="empty-state__title">No employees found</h4>
        <p className="empty-state__description">
          Try adjusting your search criteria, clearing active filters, or add a new employee.
        </p>
      </div>
    );
  }

  return (
    <div className="table-responsive">
      <table className="employee-table">
        <thead>
          <tr>
            <th onClick={() => onSort('name')} className="cursor-pointer select-none">
              <span className="th-content">
                Employee {renderSortIcon('name')}
              </span>
            </th>
            <th onClick={() => onSort('department')} className="cursor-pointer select-none">
              <span className="th-content">
                Department {renderSortIcon('department')}
              </span>
            </th>
            <th onClick={() => onSort('position')} className="cursor-pointer select-none">
              <span className="th-content">
                Position {renderSortIcon('position')}
              </span>
            </th>
            <th onClick={() => onSort('salary')} className="cursor-pointer select-none">
              <span className="th-content">
                Salary {renderSortIcon('salary')}
              </span>
            </th>
            <th onClick={() => onSort('hiredate')} className="cursor-pointer select-none">
              <span className="th-content">
                Hire Date {renderSortIcon('hiredate')}
              </span>
            </th>
            <th>Status</th>
            <th className="text-right">Actions</th>
          </tr>
        </thead>
        <tbody>
          {employees.map((emp) => (
            <tr key={emp.id} className={!emp.active ? 'row--inactive' : ''}>
              <td>
                <div className="employee-cell">
                  <div className="employee-avatar">
                    {emp.firstName.charAt(0)}
                    {emp.lastName.charAt(0)}
                  </div>
                  <div className="employee-info">
                    <span className="employee-name">{emp.fullName}</span>
                    <span className="employee-email">{emp.email}</span>
                  </div>
                </div>
              </td>
              <td>
                <span className="department-tag">{emp.department}</span>
              </td>
              <td className="text-muted">{emp.position}</td>
              <td className="font-mono">{formatCurrency(emp.salary)}</td>
              <td className="text-muted">{formatDate(emp.hireDate)}</td>
              <td>
                <StatusBadge active={emp.active} size="sm" />
              </td>
              <td className="text-right">
                <div className="action-buttons">
                  <button
                    type="button"
                    className="action-btn"
                    onClick={() => onView(emp)}
                    title="View employee details"
                    aria-label={`View details of ${emp.fullName}`}
                  >
                    <Eye size={16} />
                  </button>
                  <button
                    type="button"
                    className="action-btn"
                    onClick={() => onEdit(emp)}
                    title="Edit employee"
                    aria-label={`Edit ${emp.fullName}`}
                  >
                    <Edit2 size={16} />
                  </button>
                  <button
                    type="button"
                    className={`action-btn ${emp.active ? 'action-btn--toggle-active' : 'action-btn--toggle-inactive'}`}
                    onClick={() => onToggleStatus(emp)}
                    title={emp.active ? 'Deactivate employee' : 'Activate employee'}
                    aria-label={`Toggle active status for ${emp.fullName}`}
                  >
                    <Power size={16} />
                  </button>
                  <button
                    type="button"
                    className="action-btn action-btn--danger"
                    onClick={() => onDelete(emp)}
                    title="Delete employee"
                    aria-label={`Delete ${emp.fullName}`}
                  >
                    <Trash2 size={16} />
                  </button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
