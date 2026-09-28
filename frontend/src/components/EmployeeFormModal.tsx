import React from 'react';
import { X, UserPlus, Edit3 } from 'lucide-react';
import type { Employee, CreateEmployeeDto, UpdateEmployeeDto, ValidationProblemDetails } from '../types/employee';
import { EmployeeForm } from './EmployeeForm';

interface EmployeeFormModalProps {
  isOpen: boolean;
  employee: Employee | null;
  departments: string[];
  onClose: () => void;
  onSubmit: (data: CreateEmployeeDto | UpdateEmployeeDto) => Promise<void>;
  loading: boolean;
  serverErrors?: ValidationProblemDetails | null;
}

export const EmployeeFormModal: React.FC<EmployeeFormModalProps> = ({
  isOpen,
  employee,
  departments,
  onClose,
  onSubmit,
  loading,
  serverErrors,
}) => {
  if (!isOpen) return null;

  const isEditing = Boolean(employee);

  return (
    <div className="modal-backdrop">
      <div className="modal-dialog">
        <div className="modal-header">
          <div className="modal-title-with-icon">
            {isEditing ? <Edit3 size={20} /> : <UserPlus size={20} />}
            <h3 className="modal-title">
              {isEditing ? `Edit Employee: ${employee?.fullName}` : 'Create New Employee'}
            </h3>
          </div>
          <button
            type="button"
            className="modal-close-btn"
            onClick={onClose}
            disabled={loading}
            aria-label="Close form"
          >
            <X size={18} />
          </button>
        </div>

        <div className="modal-body">
          <EmployeeForm
            initialData={employee}
            departments={departments}
            onSubmit={onSubmit}
            onCancel={onClose}
            loading={loading}
            serverErrors={serverErrors}
          />
        </div>
      </div>
    </div>
  );
};
