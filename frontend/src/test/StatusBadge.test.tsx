import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import { StatusBadge } from '../components/StatusBadge';

describe('StatusBadge Component', () => {
  it('renders Active badge when active is true', () => {
    render(<StatusBadge active={true} />);
    const badge = screen.getByText('Active');
    expect(badge).toBeInTheDocument();
    expect(badge).toHaveClass('status-badge--active');
  });

  it('renders Inactive badge when active is false', () => {
    render(<StatusBadge active={false} />);
    const badge = screen.getByText('Inactive');
    expect(badge).toBeInTheDocument();
    expect(badge).toHaveClass('status-badge--inactive');
  });
});
