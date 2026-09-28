import React from 'react';
import { X, Mail, Briefcase, Calendar, DollarSign, Clock, Edit2 } from 'lucide-react';
import type { Employee } from '../types/employee';
import { StatusBadge } from './StatusBadge';

interface EmployeeDetailsModalProps {
  employee: Employee | null;
  isOpen: boolean;
  onClose: () => void;
  onEdit: (employee: Employee) => void;
}

export const EmployeeDetailsModal: React.FC<EmployeeDetailsModalProps> = ({
  employee,
  isOpen,
  onClose,
  onEdit,
}) => {
  if (!isOpen || !employee) return null;

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
        month: 'long',
        day: 'numeric',
      });
    } catch {
      return dateStr;
    }
  };

  const formatDateTime = (dateStr?: string | null) => {
    if (!dateStr) return '—';
    try {
      const d = new Date(dateStr);
      return d.toLocaleString('en-US', {
        dateStyle: 'medium',
        timeStyle: 'short',
      });
    } catch {
      return dateStr;
    }
  };

  const calculateTenure = (hireDateStr: string) => {
    try {
      const hire = new Date(hireDateStr);
      const now = new Date();
      const diffMs = now.getTime() - hire.getTime();
      const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
      const years = Math.floor(diffDays / 365.25);
      const months = Math.floor((diffDays % 365.25) / 30.44);

      if (years > 0) {
        return `${years} yr${years > 1 ? 's' : ''}, ${months} mo${months !== 1 ? 's' : ''}`;
      }
      return `${months} month${months !== 1 ? 's' : ''}`;
    } catch {
      return '';
    }
  };

  return (
    <div className="modal-backdrop">
      <div className="modal-dialog">
        <div className="modal-header">
          <div className="employee-details-header">
            <div className="employee-avatar employee-avatar--lg">
              {employee.firstName.charAt(0)}
              {employee.lastName.charAt(0)}
            </div>
            <div>
              <div className="flex items-center gap-2">
                <h3 className="modal-title">{employee.fullName}</h3>
                <StatusBadge active={employee.active} size="sm" />
              </div>
              <p className="employee-details-subtitle">
                {employee.position} • {employee.department}
              </p>
            </div>
          </div>

          <button
            type="button"
            className="modal-close-btn"
            onClick={onClose}
            aria-label="Close dialog"
          >
            <X size={18} />
          </button>
        </div>

        <div className="modal-body">
          <div className="details-grid">
            <div className="details-item">
              <span className="details-item__icon"><Mail size={16} /></span>
              <div>
                <span className="details-item__label">Email</span>
                <span className="details-item__value">{employee.email}</span>
              </div>
            </div>

            <div className="details-item">
              <span className="details-item__icon"><Briefcase size={16} /></span>
              <div>
                <span className="details-item__label">Department & Role</span>
                <span className="details-item__value">{employee.position} ({employee.department})</span>
              </div>
            </div>

            <div className="details-item">
              <span className="details-item__icon"><DollarSign size={16} /></span>
              <div>
                <span className="details-item__label">Compensation</span>
                <span className="details-item__value font-mono">{formatCurrency(employee.salary)} / year</span>
              </div>
            </div>

            <div className="details-item">
              <span className="details-item__icon"><Calendar size={16} /></span>
              <div>
                <span className="details-item__label">Hire Date</span>
                <span className="details-item__value">
                  {formatDate(employee.hireDate)}{' '}
                  <span className="text-muted">({calculateTenure(employee.hireDate)})</span>
                </span>
              </div>
            </div>
          </div>

          <div className="details-timestamps">
            <div className="timestamp-item">
              <Clock size={13} className="text-muted" />
              <span>Created: {formatDateTime(employee.createdAt)}</span>
            </div>
            {employee.updatedAt && (
              <div className="timestamp-item">
                <Clock size={13} className="text-muted" />
                <span>Last Updated: {formatDateTime(employee.updatedAt)}</span>
              </div>
            )}
          </div>
        </div>

        <div className="modal-footer">
          <button
            type="button"
            className="btn btn--secondary"
            onClick={onClose}
          >
            Close
          </button>
          <button
            type="button"
            className="btn btn--primary"
            onClick={() => {
              onClose();
              onEdit(employee);
            }}
          >
            <Edit2 size={16} /> Edit Employee
          </button>
        </div>
      </div>
    </div>
  );
};
