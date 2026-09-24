import {
	HttpClient,
	HttpErrorResponse,
	provideHttpClient,
	withInterceptors,
} from '@angular/common/http';
import {
	HttpTestingController,
	provideHttpClientTesting,
	TestRequest,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { firstValueFrom } from 'rxjs';
import { NotificationService } from '../../shared/notification.service';
import { errorInterceptor } from './error.interceptor';

const ENDPOINT = '/api/employees';

describe('errorInterceptor', () => {
	const notifications = { error: vi.fn() };
	let http: HttpClient;
	let httpTesting: HttpTestingController;

	beforeEach(() => {
		notifications.error.mockClear();

		TestBed.configureTestingModule({
			providers: [
				provideHttpClient(withInterceptors([errorInterceptor])),
				provideHttpClientTesting(),
				{ provide: NotificationService, useValue: notifications },
			],
		});

		http = TestBed.inject(HttpClient);
		httpTesting = TestBed.inject(HttpTestingController);
	});

	afterEach(() => {
		httpTesting.verify();
	});

	async function expectRethrown(respond: (req: TestRequest) => void): Promise<void> {
		const result = firstValueFrom(http.get(ENDPOINT));

		respond(httpTesting.expectOne(ENDPOINT));

		await expect(result).rejects.toBeInstanceOf(HttpErrorResponse);
	}

	it('should let successful responses through silently', async () => {
		const result = firstValueFrom(http.get(ENDPOINT));

		httpTesting.expectOne(ENDPOINT).flush([]);

		expect(await result).toEqual([]);
		expect(notifications.error).not.toHaveBeenCalled();
	});

	it('should show the problem detail and rethrow the error', async () => {
		await expectRethrown((req) =>
			req.flush(
				{
					status: 409,
					title: 'Conflict',
					detail: 'An employee with this email already exists.',
				},
				{ status: 409, statusText: 'Conflict' },
			),
		);

		expect(notifications.error).toHaveBeenCalledExactlyOnceWith(
			'An employee with this email already exists.',
		);
	});

	it('should show the validation errors', async () => {
		await expectRethrown((req) =>
			req.flush(
				{
					status: 400,
					title: 'One or more validation errors occurred.',
					errors: {
						Email: ["'Email' is not a valid email address."],
						Phone: ["'Phone' must not be empty."],
					},
				},
				{ status: 400, statusText: 'Bad Request' },
			),
		);

		expect(notifications.error).toHaveBeenCalledExactlyOnceWith(
			"'Email' is not a valid email address. 'Phone' must not be empty.",
		);
	});

	it('should fall back to the problem title', async () => {
		await expectRethrown((req) =>
			req.flush(
				{ status: 500, title: 'An error occurred while processing your request.' },
				{ status: 500, statusText: 'Internal Server Error' },
			),
		);

		expect(notifications.error).toHaveBeenCalledExactlyOnceWith(
			'An error occurred while processing your request.',
		);
	});

	it('should explain a network failure', async () => {
		await expectRethrown((req) => req.error(new ProgressEvent('error')));

		expect(notifications.error).toHaveBeenCalledExactlyOnceWith(
			'Cannot reach the server. Check your connection and try again.',
		);
	});

	it('should not show a response body that is not a problem', async () => {
		await expectRethrown((req) =>
			req.flush('System.InvalidOperationException: Sequence contains no elements.', {
				status: 500,
				statusText: 'Internal Server Error',
			}),
		);

		expect(notifications.error).toHaveBeenCalledExactlyOnceWith(
			'Something went wrong. Please try again.',
		);
	});
});
