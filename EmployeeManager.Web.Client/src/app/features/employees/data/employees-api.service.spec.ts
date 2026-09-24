import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Employee, EmployeeRequest, EmployeeStats } from './employee.model';
import { EmployeesApiService } from './employees-api.service';

const BASE_URL = `${environment.apiUrl}/employees`;

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

describe('EmployeesApiService', () => {
	let service: EmployeesApiService;
	let httpTesting: HttpTestingController;

	beforeEach(() => {
		TestBed.configureTestingModule({
			providers: [provideHttpClient(), provideHttpClientTesting()],
		});

		service = TestBed.inject(EmployeesApiService);
		httpTesting = TestBed.inject(HttpTestingController);
	});

	afterEach(() => {
		httpTesting.verify();
	});

	it('should not send requests until subscribed', () => {
		service.getAll();
		service.getById(employee.id);
		service.create(request);
		service.update(employee.id, request);
		service.getStats();

		httpTesting.expectNone(() => true);
	});

	it('should get all employees', async () => {
		const employees = firstValueFrom(service.getAll());

		httpTesting.expectOne({ method: 'GET', url: BASE_URL }).flush([employee]);

		expect(await employees).toEqual([employee]);
	});

	it('should get an employee by id', async () => {
		const found = firstValueFrom(service.getById(employee.id));

		httpTesting.expectOne({ method: 'GET', url: `${BASE_URL}/${employee.id}` }).flush(employee);

		expect(await found).toEqual(employee);
	});

	it('should create an employee', async () => {
		const created = firstValueFrom(service.create(request));

		const req = httpTesting.expectOne({ method: 'POST', url: BASE_URL });
		expect(req.request.body).toEqual(request);
		req.flush(employee, { status: 201, statusText: 'Created' });

		expect(await created).toEqual(employee);
	});

	it('should update an employee', async () => {
		const updated = firstValueFrom(service.update(employee.id, request));

		const req = httpTesting.expectOne({ method: 'PUT', url: `${BASE_URL}/${employee.id}` });
		expect(req.request.body).toEqual(request);
		req.flush(employee);

		expect(await updated).toEqual(employee);
	});

	it('should get the stats', async () => {
		const stats: EmployeeStats = { total: 3, active: 2, male: 1, female: 2 };
		const result = firstValueFrom(service.getStats());

		httpTesting.expectOne({ method: 'GET', url: `${BASE_URL}/stats` }).flush(stats);

		expect(await result).toEqual(stats);
	});
});
