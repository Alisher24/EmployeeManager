import { formatDateOnly, parseDateOnly } from './date-only';

describe('date-only', () => {
	it('should format the local date regardless of the time of day', () => {
		expect(formatDateOnly(new Date(1990, 4, 17))).toBe('1990-05-17');
		expect(formatDateOnly(new Date(1990, 4, 17, 23, 59))).toBe('1990-05-17');
	});

	it('should pad the month and the day', () => {
		expect(formatDateOnly(new Date(2004, 0, 5))).toBe('2004-01-05');
	});

	it('should parse the value as local midnight', () => {
		expect(parseDateOnly('1990-05-17')).toEqual(new Date(1990, 4, 17));
	});
});
