import { Component, inject, signal, viewChild } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { firstValueFrom } from 'rxjs';
import { NotificationService } from '../../../shared/notification.service';
import { EmployeeRequest } from '../data/employee.model';
import { EmployeesApiService } from '../data/employees-api.service';
import { EmployeeFormComponent } from '../employee-form/employee-form.component';

@Component({
	imports: [EmployeeFormComponent, MatCardModule],
	selector: 'app-employee-create',
	styleUrl: './employee-create.component.scss',
	templateUrl: './employee-create.component.html',
})
export class EmployeeCreateComponent {
	private readonly api = inject(EmployeesApiService);
	private readonly notifications = inject(NotificationService);
	private readonly form = viewChild.required(EmployeeFormComponent);

	protected readonly saving = signal(false);

	protected async save(request: EmployeeRequest): Promise<void> {
		this.saving.set(true);

		try {
			await firstValueFrom(this.api.create(request));
			this.notifications.success('Employee saved.');
			this.form().reset();
		} catch {

		} finally {
			this.saving.set(false);
		}
	}

	protected cancel(): void {
		this.form().reset();
		this.notifications.info('Changes discarded.');
	}
}
