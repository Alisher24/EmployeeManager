import { Component, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { EmployeeStats } from '../employees/data/employee.model';
import { EmployeesApiService } from '../employees/data/employees-api.service';

interface StatCard {
	readonly key: keyof EmployeeStats;
	readonly label: string;
	readonly icon: string;
}

@Component({
	imports: [MatButtonModule, MatCardModule, MatIconModule, MatProgressSpinnerModule],
	selector: 'app-dashboard',
	styleUrl: './dashboard.component.scss',
	templateUrl: './dashboard.component.html',
})
export class DashboardComponent {
	private readonly api = inject(EmployeesApiService);

	protected readonly cards: readonly StatCard[] = [
		{ key: 'total', label: 'Total employees', icon: 'groups' },
		{ key: 'active', label: 'Active', icon: 'how_to_reg' },
		{ key: 'male', label: 'Male', icon: 'male' },
		{ key: 'female', label: 'Female', icon: 'female' },
	];

	protected readonly stats = rxResource({ stream: () => this.api.getStats() });
}
