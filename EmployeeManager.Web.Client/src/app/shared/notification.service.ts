import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

type NotificationType = 'success' | 'info' | 'error';

const DURATION_MS = 5000;

@Injectable({ providedIn: 'root' })
export class NotificationService {
	private readonly snackBar = inject(MatSnackBar);

	success(message: string): void {
		this.show(message, 'success');
	}

	info(message: string): void {
		this.show(message, 'info');
	}

	error(message: string): void {
		this.show(message, 'error');
	}

	private show(message: string, type: NotificationType): void {
		this.snackBar.open(message, undefined, {
			duration: DURATION_MS,
			panelClass: `notification-${type}`,
		});
	}
}
