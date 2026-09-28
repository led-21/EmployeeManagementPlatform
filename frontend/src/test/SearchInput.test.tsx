import { describe, it, expect, vi } from 'vitest';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { SearchInput } from '../components/SearchInput';

describe('SearchInput Component', () => {
  it('renders input with placeholder', () => {
    render(<SearchInput value="" onChange={vi.fn()} placeholder="Search employees..." />);
    expect(screen.getByPlaceholderText('Search employees...')).toBeInTheDocument();
  });

  it('triggers debounced onChange when typing', async () => {
    const handleChange = vi.fn();
    render(<SearchInput value="" onChange={handleChange} debounceMs={50} />);

    const input = screen.getByRole('textbox');
    fireEvent.change(input, { target: { value: 'Sarah' } });

    await waitFor(() => {
      expect(handleChange).toHaveBeenCalledWith('Sarah');
    });
  });

  it('renders clear button when value is present and clears text on click', () => {
    const handleChange = vi.fn();
    render(<SearchInput value="Developer" onChange={handleChange} />);

    const clearButton = screen.getByLabelText('Clear search');
    expect(clearButton).toBeInTheDocument();

    fireEvent.click(clearButton);
    expect(handleChange).toHaveBeenCalledWith('');
  });
});
