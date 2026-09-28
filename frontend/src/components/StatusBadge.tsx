import React from 'react';

interface StatusBadgeProps {
  active: boolean;
  size?: 'sm' | 'md';
}

export const StatusBadge: React.FC<StatusBadgeProps> = ({ active, size = 'md' }) => {
  const isSm = size === 'sm';
  return (
    <span
      className={`status-badge ${active ? 'status-badge--active' : 'status-badge--inactive'} ${
        isSm ? 'status-badge--sm' : ''
      }`}
    >
      <span className="status-badge__dot" />
      {active ? 'Active' : 'Inactive'}
    </span>
  );
};
