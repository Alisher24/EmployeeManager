import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideNativeDateAdapter } from '@angular/material/core';
import { By } from '@angular/platform-browser';
import { environment } from '../../../../environments/environment';
import { NotificationService } from '../../../shared/notification.service';
import { EmployeeRequest } from '../data/employee.model';
import { EmployeeFormComponent } from '../employee-form/employee-form.component';
import { EmployeeCreateComponent } from './employee-create.component';

const EMPLOYEES_URL = `${environment.apiUrl}/employees`;

const request: EmployeeRequest = {
	firstName: 'Jane',
	lastName: 'Doe',
	dateOfBirth: '1990-05-17',
	gender: 'Female',
	address1: '221B Baker Street',
	address2: null,
	city: 'London',
	postalCode: 'NW1 6XE',
	country: 'United Kingdom',
	email: 'jane.doe@example.com',
	phone: '+447700900123',
	isActive: true,
};

describe('EmployeeCreateComponent', () => {
	const notifications = { success: vi.fn(), info: vi.fn() };
	let fixture: ComponentFixture<EmployeeCreateComponent>;
	let httpTesting: HttpTestingController;
	let form: EmployeeFormComponent;

	beforeEach(async () => {
		notifications.success.mockClear();
		notifications.info.mockClear();

		await TestBed.configureTestingModule({
			imports: [EmployeeCreateComponent],
			providers: [
				provideHttpClient(),
				provideHttpClientTesting(),
				provideNativeDateAdapter(),
				{ provide: NotificationService, useValue: notifications },
			],
		}).compileComponents();

		httpTesting = TestBed.inject(HttpTestingController);
		fixture = TestBed.createComponent(EmployeeCreateComponent);
		await fixture.whenStable();

		form = fixture.debugElement
			.query(By.directive(EmployeeFormComponent))
			.injector.get(EmployeeFormComponent);
		vi.spyOn(form, 'reset');
	});

	afterEach(() => {
		httpTesting.verify();
	});

	function isShowingProgress(): boolean {
		const saveButton: HTMLButtonElement =
			fixture.nativeElement.querySelector('button[type="submit"]');

		return saveButton.querySelector('mat-spinner') !== null;
	}

	it('should create the employee, confirm it and reset the form', async () => {
		form.saved.emit(request);

		const req = httpTesting.expectOne({ method: 'POST', url: EMPLOYEES_URL });
		expect(req.request.body).toEqual(request);
		req.flush({ id: '0f8fad5b-d9cb-469f-a165-70867728950e', ...request });
		await fixture.whenStable();

		expect(notifications.success).toHaveBeenCalledExactlyOnceWith('Employee saved.');
		expect(form.reset).toHaveBeenCalledOnce();
	});

	it('should show progress on Save until the request completes', async () => {
		form.saved.emit(request);
		fixture.detectChanges();

		expect(isShowingProgress()).toBe(true);

		httpTesting
			.expectOne(EMPLOYEES_URL)
			.flush(null, { status: 500, statusText: 'Server Error' });
		await fixture.whenStable();
		fixture.detectChanges();

		expect(isShowingProgress()).toBe(false);
	});

	it('should keep the entered data when saving fails', async () => {
		form.saved.emit(request);

		httpTesting
			.expectOne(EMPLOYEES_URL)
			.flush(
				{ status: 409, detail: 'An employee with this email already exists.' },
				{ status: 409, statusText: 'Conflict' },
			);
		await fixture.whenStable();

		expect(notifications.success).not.toHaveBeenCalled();
		expect(form.reset).not.toHaveBeenCalled();
	});

	it('should reset the form and say so on Cancel', () => {
		form.cancelled.emit();

		expect(form.reset).toHaveBeenCalledOnce();
		expect(notifications.info).toHaveBeenCalledExactlyOnceWith('Changes discarded.');
	});
});
