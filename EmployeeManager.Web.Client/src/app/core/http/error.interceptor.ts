import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../../shared/notification.service';

interface ProblemDetails {
	readonly title?: string;
	readonly detail?: string;
	readonly errors?: Record<string, string[]>;
}

const NETWORK_ERROR_MESSAGE = 'Cannot reach the server. Check your connection and try again.';
const NOT_FOUND_MESSAGE = 'The requested data was not found.';
const FALLBACK_MESSAGE = 'Something went wrong. Please try again.';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
	const notifications = inject(NotificationService);

	return next(req).pipe(
		catchError((error: unknown) => {
			if (error instanceof HttpErrorResponse) {
				notifications.error(getErrorMessage(error));
			}

			return throwError(() => error);
		}),
	);
};

function getErrorMessage(error: HttpErrorResponse): string {
	if (error.status === 0) {
		return NETWORK_ERROR_MESSAGE;
	}

	const problem: ProblemDetails | null = typeof error.error === 'object' ? error.error : null;
	const validationErrors = Object.values(problem?.errors ?? {}).flat();

	if (validationErrors.length > 0) {
		return validationErrors.join(' ');
	}

	if (problem?.detail) {
		return problem.detail;
	}

	if (error.status === 404) {
		return NOT_FOUND_MESSAGE;
	}

	return problem?.title || FALLBACK_MESSAGE;
}
