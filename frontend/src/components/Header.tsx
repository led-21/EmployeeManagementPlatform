import React from 'react';
import { Briefcase, UserPlus } from 'lucide-react';

interface HeaderProps {
  onAddClick: () => void;
}

export const Header: React.FC<HeaderProps> = ({ onAddClick }) => {
  return (
    <header className="app-header">
      <div className="header-inner">
        <div className="brand-wrapper">
          <div className="brand-icon">
            <Briefcase size={22} />
          </div>
          <div>
            <h1 className="brand-title">EmployeeManagementPlatform</h1>
            <p className="brand-subtitle">Company workforce directory & organization metrics</p>
          </div>
        </div>

        <button
          type="button"
          className="btn btn--primary"
          onClick={onAddClick}
        >
          <UserPlus size={18} />
          <span>Add Employee</span>
        </button>
      </div>
    </header>
  );
};
