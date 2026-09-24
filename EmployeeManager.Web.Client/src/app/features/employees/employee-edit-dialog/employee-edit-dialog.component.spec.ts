import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatInputHarness } from '@angular/material/input/testing';
import { By } from '@angular/platform-browser';
import { Subject } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Employee, EmployeeRequest } from '../data/employee.model';
import { EmployeeFormComponent } from '../employee-form/employee-form.component';
import { EmployeeEditDialogComponent } from './employee-edit-dialog.component';

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

const employee: Employee = { id: '0f8fad5b-d9cb-469f-a165-70867728950e', ...request };
const changes: EmployeeRequest = { ...request, city: 'Oxford' };

const EMPLOYEE_URL = `${environment.apiUrl}/employees/${employee.id}`;

describe('EmployeeEditDialogComponent', () => {
	const keydownEvents = new Subject<KeyboardEvent>();
	const dialogRef = { close: vi.fn(), disableClose: false, keydownEvents: () => keydownEvents };
	let fixture: ComponentFixture<EmployeeEditDialogComponent>;
	let httpTesting: HttpTestingController;
	let loader: HarnessLoader;
	let form: EmployeeFormComponent;

	beforeEach(async () => {
		dialogRef.close.mockClear();
		dialogRef.disableClose = false;

		await TestBed.configureTestingModule({
			imports: [EmployeeEditDialogComponent],
			providers: [
				provideHttpClient(),
				provideHttpClientTesting(),
				provideNativeDateAdapter(),
				{ provide: MAT_DIALOG_DATA, useValue: employee },
				{ provide: MatDialogRef, useValue: dialogRef },
			],
		}).compileComponents();

		httpTesting = TestBed.inject(HttpTestingController);
		fixture = TestBed.createComponent(EmployeeEditDialogComponent);
		loader = TestbedHarnessEnvironment.loader(fixture);
		await fixture.whenStable();

		form = fixture.debugElement
			.query(By.directive(EmployeeFormComponent))
			.injector.get(EmployeeFormComponent);
	});

	afterEach(() => {
		httpTesting.verify();
	});

	function saveButton(): HTMLButtonElement {
		return fixture.nativeElement.querySelector('button[type="submit"]');
	}

	function pressEscape(): void {
		keydownEvents.next(new KeyboardEvent('keydown', { key: 'Escape' }));
	}

	it('should fill in the form with the employee', async () => {
		const firstName = await loader.getHarness(MatInputHarness.with({ label: 'First name' }));
		const email = await loader.getHarness(MatInputHarness.with({ label: 'Email' }));

		expect(await firstName.getValue()).toBe('Jane');
		expect(await email.getValue()).toBe('jane.doe@example.com');
	});

	it('should update the employee and close with the result', async () => {
		const updated: Employee = { ...employee, ...changes };

		form.saved.emit(changes);

		const req = httpTesting.expectOne({ method: 'PUT', url: EMPLOYEE_URL });
		expect(req.request.body).toEqual(changes);
		req.flush(updated);
		await fixture.whenStable();

		expect(dialogRef.close).toHaveBeenCalledExactlyOnceWith(updated);
	});

	it('should stay open when saving fails', async () => {
		form.saved.emit(changes);

		httpTesting
			.expectOne(EMPLOYEE_URL)
			.flush(
				{ status: 409, detail: 'An employee with this email already exists.' },
				{ status: 409, statusText: 'Conflict' },
			);
		await fixture.whenStable();

		expect(dialogRef.close).not.toHaveBeenCalled();
	});

	it('should block Save and Escape until the request completes', async () => {
		form.saved.emit(changes);
		fixture.detectChanges();
		pressEscape();

		expect(saveButton().disabled).toBe(true);
		expect(dialogRef.close).not.toHaveBeenCalled();

		httpTesting
			.expectOne(EMPLOYEE_URL)
			.flush(null, { status: 500, statusText: 'Internal Server Error' });
		await fixture.whenStable();
		fixture.detectChanges();

		expect(saveButton().disabled).toBe(false);
	});

	it('should close without a result on Cancel', () => {
		form.cancelled.emit();

		expect(dialogRef.close).toHaveBeenCalledExactlyOnceWith();
	});

	it('should close without a result on Escape', () => {
		pressEscape();

		expect(dialogRef.close).toHaveBeenCalledExactlyOnceWith();
	});

	it('should not close on a backdrop click', () => {
		expect(dialogRef.disableClose).toBe(true);
	});
});
