import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { environment } from '../../../environments/environment';
import { EmployeeStats } from '../employees/data/employee.model';
import { DashboardComponent } from './dashboard.component';

const STATS_URL = `${environment.apiUrl}/employees/stats`;

const stats: EmployeeStats = { total: 20, active: 15, male: 11, female: 9 };

describe('DashboardComponent', () => {
	let fixture: ComponentFixture<DashboardComponent>;
	let host: HTMLElement;
	let httpTesting: HttpTestingController;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [DashboardComponent],
			providers: [provideHttpClient(), provideHttpClientTesting()],
		}).compileComponents();

		httpTesting = TestBed.inject(HttpTestingController);
		fixture = TestBed.createComponent(DashboardComponent);
		host = fixture.nativeElement;

		fixture.detectChanges();
	});

	afterEach(() => {
		httpTesting.verify();
	});

	function texts(selector: string): (string | null)[] {
		return Array.from(host.querySelectorAll(selector), (element) => element.textContent);
	}

	it('should show a spinner while the stats load', async () => {
		expect(host.querySelector('mat-spinner')).not.toBeNull();

		httpTesting.expectOne(STATS_URL).flush(stats);
		await fixture.whenStable();

		expect(host.querySelector('mat-spinner')).toBeNull();
	});

	it('should show a card for each stat', async () => {
		httpTesting.expectOne(STATS_URL).flush(stats);
		await fixture.whenStable();

		expect(texts('.stat-label')).toEqual(['Total employees', 'Active', 'Male', 'Female']);
		expect(texts('.stat-value')).toEqual(['20', '15', '11', '9']);
	});

	it('should load the stats again after a failure', async () => {
		httpTesting
			.expectOne(STATS_URL)
			.flush(null, { status: 500, statusText: 'Internal Server Error' });
		await fixture.whenStable();

		expect(host.querySelector('mat-card')).toBeNull();
		expect(host.textContent).toContain("Couldn't load the statistics.");

		host.querySelector('button')?.click();
		fixture.detectChanges();

		expect(host.querySelector('mat-spinner')).not.toBeNull();

		httpTesting.expectOne(STATS_URL).flush(stats);
		await fixture.whenStable();

		expect(texts('.stat-value')).toEqual(['20', '15', '11', '9']);
	});
});
