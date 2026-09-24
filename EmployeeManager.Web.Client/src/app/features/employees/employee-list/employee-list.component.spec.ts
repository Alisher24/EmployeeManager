import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginatorHarness } from '@angular/material/paginator/testing';
import { MatSortHeaderHarness } from '@angular/material/sort/testing';
import { MatTableHarness } from '@angular/material/table/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { NotificationService } from '../../../shared/notification.service';
import { Employee } from '../data/employee.model';
import { EmployeeEditDialogComponent } from '../employee-edit-dialog/employee-edit-dialog.component';
import { EmployeeListComponent } from './employee-list.component';

const EMPLOYEES_URL = `${environment.apiUrl}/employees`;

const jane: Employee = {
	id: '0f8fad5b-d9cb-469f-a165-70867728950e',
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

const john: Employee = {
	...jane,
	id: '7c9e6679-7425-40de-944b-e07fc1f90ae7',
	firstName: 'John',
	lastName: 'Smith',
	gender: 'Male',
	city: 'Manchester',
	email: 'john.smith@example.com',
	phone: '+447700900456',
	isActive: false,
};

describe('EmployeeListComponent', () => {
	const notifications = { success: vi.fn(), info: vi.fn() };
	const dialog = { open: vi.fn() };
	let fixture: ComponentFixture<EmployeeListComponent>;
	let host: HTMLElement;
	let httpTesting: HttpTestingController;
	let loader: HarnessLoader;

	beforeEach(async () => {
		notifications.success.mockClear();
		notifications.info.mockClear();
		dialog.open.mockReset();

		await TestBed.configureTestingModule({
			imports: [EmployeeListComponent],
			providers: [
				provideHttpClient(),
				provideHttpClientTesting(),
				provideRouter([]),
				{ provide: NotificationService, useValue: notifications },
				{ provide: MatDialog, useValue: dialog },
			],
		}).compileComponents();

		httpTesting = TestBed.inject(HttpTestingController);
		fixture = TestBed.createComponent(EmployeeListComponent);
		host = fixture.nativeElement;
		loader = TestbedHarnessEnvironment.loader(fixture);

		fixture.detectChanges();
	});

	afterEach(() => {
		httpTesting.verify();
	});

	async function load(employees: Employee[]): Promise<void> {
		httpTesting.expectOne(EMPLOYEES_URL).flush(employees);
		await fixture.whenStable();
	}

	async function getColumn(name: string): Promise<string[]> {
		const table = await loader.getHarness(MatTableHarness);
		const columns = await table.getCellTextByColumnName();

		return columns[name].text;
	}

	function rows(): HTMLTableRowElement[] {
		return Array.from(host.querySelectorAll('tr[mat-row]'));
	}

	function closeDialogWith(result: Employee | undefined): void {
		dialog.open.mockReturnValue({ afterClosed: () => of(result) });
	}

	it('should show a spinner while the employees load', async () => {
		expect(host.querySelector('mat-spinner')).not.toBeNull();

		await load([jane]);

		expect(host.querySelector('mat-spinner')).toBeNull();
	});

	it('should show a row for each employee', async () => {
		await load([jane, john]);

		const table = await loader.getHarness(MatTableHarness);

		expect(await table.getCellTextByIndex()).toEqual([
			['Jane', 'Doe', 'jane.doe@example.com', '+447700900123', 'London', 'Female', 'Active'],
			[
				'John',
				'Smith',
				'john.smith@example.com',
				'+447700900456',
				'Manchester',
				'Male',
				'Inactive',
			],
		]);
	});

	it('should sort the employees by a column', async () => {
		await load([john, jane]);

		const firstName = await loader.getHarness(
			MatSortHeaderHarness.with({ label: 'First name' }),
		);

		await firstName.click();
		expect(await getColumn('firstName')).toEqual(['Jane', 'John']);

		await firstName.click();
		expect(await getColumn('firstName')).toEqual(['John', 'Jane']);
	});

	it('should show the employees 10 per page', async () => {
		await load(
			Array.from({ length: 12 }, (_, i) => ({
				...jane,
				id: String(i),
				firstName: `Employee ${i + 1}`,
			})),
		);

		const paginator = await loader.getHarness(MatPaginatorHarness);

		expect(await paginator.getRangeLabel()).toBe('1 – 10 of 12');
		expect(await getColumn('firstName')).toHaveLength(10);

		await paginator.goToNextPage();

		expect(await getColumn('firstName')).toEqual(['Employee 11', 'Employee 12']);
	});

	it('should say that there are no employees yet', async () => {
		await load([]);

		expect(host.querySelector('table')).toBeNull();
		expect(host.textContent).toContain('No employees yet.');
		expect(host.querySelector('a')?.getAttribute('href')).toBe('/employees/new');
	});

	it('should load the employees again after a failure', async () => {
		httpTesting
			.expectOne(EMPLOYEES_URL)
			.flush(null, { status: 500, statusText: 'Internal Server Error' });
		await fixture.whenStable();

		expect(host.querySelector('table')).toBeNull();
		expect(host.textContent).toContain("Couldn't load the employees.");

		host.querySelector('button')?.click();
		fixture.detectChanges();

		expect(host.querySelector('mat-spinner')).not.toBeNull();

		await load([jane]);

		expect(await getColumn('firstName')).toEqual(['Jane']);
	});

	it('should open the clicked employee in the edit dialog', async () => {
		closeDialogWith(undefined);
		await load([jane, john]);

		rows()[1].click();

		expect(dialog.open).toHaveBeenCalledExactlyOnceWith(
			EmployeeEditDialogComponent,
			expect.objectContaining({ data: john }),
		);
	});

	it('should open the edit dialog with Enter', async () => {
		closeDialogWith(undefined);
		await load([jane]);

		rows()[0].dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter' }));

		expect(dialog.open).toHaveBeenCalledExactlyOnceWith(
			EmployeeEditDialogComponent,
			expect.objectContaining({ data: jane }),
		);
	});

	it('should reload the employees and confirm the saved changes', async () => {
		const updated: Employee = { ...jane, city: 'Oxford' };
		closeDialogWith(updated);
		await load([jane]);

		rows()[0].click();

		const reload = await vi.waitFor(() => httpTesting.expectOne(EMPLOYEES_URL));
		reload.flush([updated]);
		await fixture.whenStable();

		expect(notifications.success).toHaveBeenCalledExactlyOnceWith('Changes saved.');
		expect(await getColumn('city')).toEqual(['Oxford']);
	});

	it('should keep the table and show progress while reloading', async () => {
		const updated: Employee = { ...jane, city: 'Oxford' };
		closeDialogWith(updated);
		await load([jane]);

		rows()[0].click();

		const reload = await vi.waitFor(() => httpTesting.expectOne(EMPLOYEES_URL));
		fixture.detectChanges();

		expect(host.querySelector('mat-progress-bar')).not.toBeNull();
		expect(rows()).toHaveLength(1);

		reload.flush([updated]);
		await fixture.whenStable();

		expect(host.querySelector('mat-progress-bar')).toBeNull();
	});

	it('should say that editing was cancelled', async () => {
		closeDialogWith(undefined);
		await load([jane]);

		rows()[0].click();
		await fixture.whenStable();

		expect(notifications.info).toHaveBeenCalledExactlyOnceWith('Editing cancelled.');
		expect(notifications.success).not.toHaveBeenCalled();
		httpTesting.expectNone(EMPLOYEES_URL);
	});
});
