import React from 'react';
import { Users, UserCheck, UserX, Building2 } from 'lucide-react';
import type { DashboardSummary } from '../types/employee';

interface SummaryCardsProps {
  summary: DashboardSummary | null;
  loading: boolean;
}

export const SummaryCards: React.FC<SummaryCardsProps> = ({ summary, loading }) => {
  if (loading || !summary) {
    return (
      <div className="summary-grid">
        {[1, 2, 3, 4].map((i) => (
          <div key={i} className="summary-card summary-card--skeleton">
            <div className="skeleton-line skeleton-line--sm" />
            <div className="skeleton-line skeleton-line--lg" />
          </div>
        ))}
      </div>
    );
  }

  return (
    <div className="summary-grid">
      <div className="summary-card">
        <div className="summary-card__icon summary-card__icon--blue">
          <Users size={22} />
        </div>
        <div className="summary-card__content">
          <span className="summary-card__label">Total Employees</span>
          <span className="summary-card__value">{summary.totalEmployees}</span>
        </div>
      </div>

      <div className="summary-card">
        <div className="summary-card__icon summary-card__icon--emerald">
          <UserCheck size={22} />
        </div>
        <div className="summary-card__content">
          <span className="summary-card__label">Active Workforce</span>
          <span className="summary-card__value">{summary.activeEmployees}</span>
        </div>
      </div>

      <div className="summary-card">
        <div className="summary-card__icon summary-card__icon--amber">
          <UserX size={22} />
        </div>
        <div className="summary-card__content">
          <span className="summary-card__label">Inactive</span>
          <span className="summary-card__value">{summary.inactiveEmployees}</span>
        </div>
      </div>

      <div className="summary-card">
        <div className="summary-card__icon summary-card__icon--indigo">
          <Building2 size={22} />
        </div>
        <div className="summary-card__content">
          <span className="summary-card__label">Departments</span>
          <span className="summary-card__value">{summary.totalDepartments}</span>
        </div>
      </div>
    </div>
  );
};
