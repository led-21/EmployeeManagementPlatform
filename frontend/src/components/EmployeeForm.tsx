import React, { useState } from 'react';
import type { CreateEmployeeDto, UpdateEmployeeDto, Employee, ValidationProblemDetails } from '../types/employee';

interface EmployeeFormProps {
  initialData?: Employee | null;
  departments: string[];
  onSubmit: (data: CreateEmployeeDto | UpdateEmployeeDto) => Promise<void>;
  onCancel: () => void;
  loading: boolean;
  serverErrors?: ValidationProblemDetails | null;
}

export const EmployeeForm: React.FC<EmployeeFormProps> = ({
  initialData,
  departments,
  onSubmit,
  onCancel,
  loading,
  serverErrors,
}) => {
  const isEditing = Boolean(initialData);

  // Form State
  const [firstName, setFirstName] = useState(initialData?.firstName || '');
  const [lastName, setLastName] = useState(initialData?.lastName || '');
  const [email, setEmail] = useState(initialData?.email || '');
  const [position, setPosition] = useState(initialData?.position || '');
  const [department, setDepartment] = useState(initialData?.department || '');
  const [customDepartment, setCustomDepartment] = useState('');
  const [salary, setSalary] = useState(initialData ? String(initialData.salary) : '');
  const [hireDate, setHireDate] = useState(
    initialData?.hireDate ? initialData.hireDate.substring(0, 10) : new Date().toISOString().substring(0, 10)
  );
  const [active, setActive] = useState(initialData ? initialData.active : true);

  // Client Validation Errors
  const [errors, setErrors] = useState<Record<string, string>>({});

  const validate = (): boolean => {
    const errs: Record<string, string> = {};

    if (!firstName.trim()) {
      errs.firstName = 'First name is required.';
    } else if (firstName.trim().length > 50) {
      errs.firstName = 'First name cannot exceed 50 characters.';
    }

    if (!lastName.trim()) {
      errs.lastName = 'Last name is required.';
    } else if (lastName.trim().length > 50) {
      errs.lastName = 'Last name cannot exceed 50 characters.';
    } else if (firstName.trim().toLowerCase() === lastName.trim().toLowerCase()) {
      errs.lastName = 'First name and last name cannot be identical.';
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!email.trim()) {
      errs.email = 'Email is required.';
    } else if (!emailRegex.test(email.trim())) {
      errs.email = 'Enter a valid email address.';
    }

    if (!position.trim()) {
      errs.position = 'Position is required.';
    }

    const effectiveDept = department === '__custom__' ? customDepartment.trim() : department.trim();
    if (!effectiveDept) {
      errs.department = 'Department is required.';
    }

    const numSalary = Number(salary);
    if (salary === '' || isNaN(numSalary)) {
      errs.salary = 'Valid salary amount is required.';
    } else if (numSalary < 0) {
      errs.salary = 'Salary must be non-negative.';
    }

    if (!hireDate) {
      errs.hireDate = 'Hire date is required.';
    } else {
      const selectedDate = new Date(hireDate);
      const today = new Date();
      today.setHours(23, 59, 59, 999);
      if (selectedDate > today) {
        errs.hireDate = 'Hire date cannot be in the future.';
      }
    }

    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    const effectiveDept = department === '__custom__' ? customDepartment.trim() : department.trim();

    if (isEditing) {
      const dto: UpdateEmployeeDto = {
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        email: email.trim(),
        position: position.trim(),
        department: effectiveDept,
        salary: Number(salary),
        hireDate: new Date(hireDate).toISOString(),
        active,
      };
      await onSubmit(dto);
    } else {
      const dto: CreateEmployeeDto = {
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        email: email.trim(),
        position: position.trim(),
        department: effectiveDept,
        salary: Number(salary),
        hireDate: new Date(hireDate).toISOString(),
      };
      await onSubmit(dto);
    }
  };

  const getServerError = (field: string) => {
    return serverErrors?.errors?.[field]?.[0] || serverErrors?.errors?.[field.charAt(0).toUpperCase() + field.slice(1)]?.[0];
  };

  return (
    <form onSubmit={handleSubmit} className="employee-form" noValidate>
      {serverErrors?.detail && (
        <div className="form-alert form-alert--danger">
          {serverErrors.detail}
        </div>
      )}

      <div className="form-grid">
        <div className="form-group">
          <label className="form-label" htmlFor="firstName">
            First Name <span className="text-danger">*</span>
          </label>
          <input
            id="firstName"
            type="text"
            className={`form-input ${errors.firstName || getServerError('firstName') ? 'form-input--error' : ''}`}
            placeholder="e.g. John"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            disabled={loading}
          />
          {(errors.firstName || getServerError('firstName')) && (
            <span className="form-error">{errors.firstName || getServerError('firstName')}</span>
          )}
        </div>

        <div className="form-group">
          <label className="form-label" htmlFor="lastName">
            Last Name <span className="text-danger">*</span>
          </label>
          <input
            id="lastName"
            type="text"
            className={`form-input ${errors.lastName || getServerError('lastName') ? 'form-input--error' : ''}`}
            placeholder="e.g. Doe"
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
            disabled={loading}
          />
          {(errors.lastName || getServerError('lastName')) && (
            <span className="form-error">{errors.lastName || getServerError('lastName')}</span>
          )}
        </div>
      </div>

      <div className="form-grid">
        <div className="form-group">
          <label className="form-label" htmlFor="email">
            Corporate Email <span className="text-danger">*</span>
          </label>
          <input
            id="email"
            type="email"
            className={`form-input ${errors.email || getServerError('email') ? 'form-input--error' : ''}`}
            placeholder="e.g. john.doe@company.com"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            disabled={loading}
          />
          {(errors.email || getServerError('email')) && (
            <span className="form-error">{errors.email || getServerError('email')}</span>
          )}
        </div>

        <div className="form-group">
          <label className="form-label" htmlFor="position">
            Position / Job Title <span className="text-danger">*</span>
          </label>
          <input
            id="position"
            type="text"
            className={`form-input ${errors.position || getServerError('position') ? 'form-input--error' : ''}`}
            placeholder="e.g. Senior Software Engineer"
            value={position}
            onChange={(e) => setPosition(e.target.value)}
            disabled={loading}
          />
          {(errors.position || getServerError('position')) && (
            <span className="form-error">{errors.position || getServerError('position')}</span>
          )}
        </div>
      </div>

      <div className="form-grid">
        <div className="form-group">
          <label className="form-label" htmlFor="department">
            Department <span className="text-danger">*</span>
          </label>
          <select
            id="department"
            className={`form-select ${errors.department || getServerError('department') ? 'form-input--error' : ''}`}
            value={department}
            onChange={(e) => setDepartment(e.target.value)}
            disabled={loading}
          >
            <option value="">Select department...</option>
            {departments.map((dept) => (
              <option key={dept} value={dept}>
                {dept}
              </option>
            ))}
            <option value="__custom__">+ Enter new department</option>
          </select>

          {department === '__custom__' && (
            <input
              type="text"
              className="form-input form-input--nested mt-2"
              placeholder="Enter department name"
              value={customDepartment}
              onChange={(e) => setCustomDepartment(e.target.value)}
              disabled={loading}
            />
          )}

          {(errors.department || getServerError('department')) && (
            <span className="form-error">{errors.department || getServerError('department')}</span>
          )}
        </div>

        <div className="form-group">
          <label className="form-label" htmlFor="salary">
            Annual Salary (USD) <span className="text-danger">*</span>
          </label>
          <input
            id="salary"
            type="number"
            min="0"
            step="1000"
            className={`form-input ${errors.salary || getServerError('salary') ? 'form-input--error' : ''}`}
            placeholder="e.g. 75000"
            value={salary}
            onChange={(e) => setSalary(e.target.value)}
            disabled={loading}
          />
          {(errors.salary || getServerError('salary')) && (
            <span className="form-error">{errors.salary || getServerError('salary')}</span>
          )}
        </div>
      </div>

      <div className="form-grid">
        <div className="form-group">
          <label className="form-label" htmlFor="hireDate">
            Hire Date <span className="text-danger">*</span>
          </label>
          <input
            id="hireDate"
            type="date"
            className={`form-input ${errors.hireDate || getServerError('hireDate') ? 'form-input--error' : ''}`}
            value={hireDate}
            onChange={(e) => setHireDate(e.target.value)}
            disabled={loading}
          />
          {(errors.hireDate || getServerError('hireDate')) && (
            <span className="form-error">{errors.hireDate || getServerError('hireDate')}</span>
          )}
        </div>

        {isEditing && (
          <div className="form-group form-group--checkbox">
            <label className="checkbox-label" htmlFor="activeStatus">
              <input
                id="activeStatus"
                type="checkbox"
                checked={active}
                onChange={(e) => setActive(e.target.checked)}
                disabled={loading}
              />
              <span>Employee is currently active</span>
            </label>
            <span className="form-hint">Uncheck to mark as inactive/deactivated.</span>
          </div>
        )}
      </div>

      <div className="form-actions">
        <button
          type="button"
          className="btn btn--secondary"
          onClick={onCancel}
          disabled={loading}
        >
          Cancel
        </button>
        <button
          type="submit"
          className="btn btn--primary"
          disabled={loading}
        >
          {loading ? 'Saving...' : isEditing ? 'Save Changes' : 'Create Employee'}
        </button>
      </div>
    </form>
  );
};
