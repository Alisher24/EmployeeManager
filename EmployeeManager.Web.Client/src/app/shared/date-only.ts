export function formatDateOnly(date: Date): string {
	const year = String(date.getFullYear()).padStart(4, '0');
	const month = String(date.getMonth() + 1).padStart(2, '0');
	const day = String(date.getDate()).padStart(2, '0');

	return `${year}-${month}-${day}`;
}
export function parseDateOnly(value: string): Date {
	const [year, month, day] = value.split('-').map(Number);

	return new Date(year, month - 1, day);
}
