import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { Pagination } from '../components/Pagination';

describe('Pagination Component', () => {
  it('displays accurate record ranges', () => {
    const { container } = render(
      <Pagination
        currentPage={2}
        totalPages={5}
        totalCount={48}
        pageSize={10}
        onPageChange={vi.fn()}
        onPageSizeChange={vi.fn()}
      />
    );

    const info = container.querySelector('.pagination__info');
    expect(info).toHaveTextContent('Showing 11 to 20 of 48 employees');
  });

  it('disables previous button on first page and allows next page navigation', () => {
    const handlePageChange = vi.fn();
    render(
      <Pagination
        currentPage={1}
        totalPages={3}
        totalCount={25}
        pageSize={10}
        onPageChange={handlePageChange}
        onPageSizeChange={vi.fn()}
      />
    );

    const prevButton = screen.getByLabelText('Previous page');
    const nextButton = screen.getByLabelText('Next page');

    expect(prevButton).toBeDisabled();
    expect(nextButton).not.toBeDisabled();

    fireEvent.click(nextButton);
    expect(handlePageChange).toHaveBeenCalledWith(2);
  });

  it('triggers onPageSizeChange when page size dropdown is changed', () => {
    const handleSizeChange = vi.fn();
    render(
      <Pagination
        currentPage={1}
        totalPages={2}
        totalCount={20}
        pageSize={10}
        onPageChange={vi.fn()}
        onPageSizeChange={handleSizeChange}
      />
    );

    const select = screen.getByRole('combobox');
    fireEvent.change(select, { target: { value: '20' } });

    expect(handleSizeChange).toHaveBeenCalledWith(20);
  });
});
