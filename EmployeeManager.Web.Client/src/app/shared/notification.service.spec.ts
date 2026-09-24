import { TestBed } from '@angular/core/testing';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NotificationService } from './notification.service';

describe('NotificationService', () => {
	const snackBar = { open: vi.fn() };
	let service: NotificationService;

	beforeEach(() => {
		snackBar.open.mockClear();

		TestBed.configureTestingModule({
			providers: [{ provide: MatSnackBar, useValue: snackBar }],
		});

		service = TestBed.inject(NotificationService);
	});

	it.each(['success', 'info', 'error'] as const)('should show %s for 5 seconds', (type) => {
		service[type]('Test message');

		expect(snackBar.open).toHaveBeenCalledExactlyOnceWith('Test message', undefined, {
			duration: 5000,
			panelClass: `notification-${type}`,
		});
	});
});
