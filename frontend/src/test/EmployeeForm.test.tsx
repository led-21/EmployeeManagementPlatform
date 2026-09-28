import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { EmployeeForm } from '../components/EmployeeForm';

describe('EmployeeForm Component', () => {
  const departments = ['Engineering', 'Product', 'Finance'];

  it('renders all required form fields', () => {
    render(
      <EmployeeForm
        departments={departments}
        onSubmit={vi.fn()}
        onCancel={vi.fn()}
        loading={false}
      />
    );

    expect(screen.getByLabelText(/First Name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Last Name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Corporate Email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Position/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Department/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Annual Salary/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Hire Date/i)).toBeInTheDocument();
  });

  it('shows validation errors when submitting an empty form', async () => {
    const handleSubmit = vi.fn();
    render(
      <EmployeeForm
        departments={departments}
        onSubmit={handleSubmit}
        onCancel={vi.fn()}
        loading={false}
      />
    );

    const submitBtn = screen.getByRole('button', { name: /Create Employee/i });
    fireEvent.click(submitBtn);

    expect(await screen.findByText('First name is required.')).toBeInTheDocument();
    expect(screen.getByText('Last name is required.')).toBeInTheDocument();
    expect(screen.getByText('Email is required.')).toBeInTheDocument();
    expect(screen.getByText('Position is required.')).toBeInTheDocument();
    expect(handleSubmit).not.toHaveBeenCalled();
  });

  it('validates that first name and last name cannot be identical', async () => {
    render(
      <EmployeeForm
        departments={departments}
        onSubmit={vi.fn()}
        onCancel={vi.fn()}
        loading={false}
      />
    );

    fireEvent.change(screen.getByLabelText(/First Name/i), { target: { value: 'Smith' } });
    fireEvent.change(screen.getByLabelText(/Last Name/i), { target: { value: 'Smith' } });

    fireEvent.click(screen.getByRole('button', { name: /Create Employee/i }));

    expect(
      await screen.findByText('First name and last name cannot be identical.')
    ).toBeInTheDocument();
  });

  it('validates email format', async () => {
    render(
      <EmployeeForm
        departments={departments}
        onSubmit={vi.fn()}
        onCancel={vi.fn()}
        loading={false}
      />
    );

    fireEvent.change(screen.getByLabelText(/Corporate Email/i), { target: { value: 'not-an-email' } });
    fireEvent.click(screen.getByRole('button', { name: /Create Employee/i }));

    expect(await screen.findByText('Enter a valid email address.')).toBeInTheDocument();
  });

  it('submits valid data successfully', async () => {
    const handleSubmit = vi.fn().mockResolvedValue(undefined);
    render(
      <EmployeeForm
        departments={departments}
        onSubmit={handleSubmit}
        onCancel={vi.fn()}
        loading={false}
      />
    );

    fireEvent.change(screen.getByLabelText(/First Name/i), { target: { value: 'Ada' } });
    fireEvent.change(screen.getByLabelText(/Last Name/i), { target: { value: 'Lovelace' } });
    fireEvent.change(screen.getByLabelText(/Corporate Email/i), { target: { value: 'ada@example.com' } });
    fireEvent.change(screen.getByLabelText(/Position/i), { target: { value: 'Chief Scientist' } });
    fireEvent.change(screen.getByLabelText(/Department/i), { target: { value: 'Engineering' } });
    fireEvent.change(screen.getByLabelText(/Annual Salary/i), { target: { value: '110000' } });
    fireEvent.change(screen.getByLabelText(/Hire Date/i), { target: { value: '2022-05-10' } });

    fireEvent.click(screen.getByRole('button', { name: /Create Employee/i }));

    await waitFor(() => {
      expect(handleSubmit).toHaveBeenCalledTimes(1);
      const callArg = handleSubmit.mock.calls[0][0];
      expect(callArg.firstName).toBe('Ada');
      expect(callArg.lastName).toBe('Lovelace');
      expect(callArg.email).toBe('ada@example.com');
      expect(callArg.salary).toBe(110000);
      expect(callArg.department).toBe('Engineering');
    });
  });
});
