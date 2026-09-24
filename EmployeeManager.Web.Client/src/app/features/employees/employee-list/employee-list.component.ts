import { Component, effect, inject, TrackByFunction, viewChild } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { NotificationService } from '../../../shared/notification.service';
import { Employee } from '../data/employee.model';
import { EmployeesApiService } from '../data/employees-api.service';
import { EmployeeEditDialogComponent } from '../employee-edit-dialog/employee-edit-dialog.component';

@Component({
	imports: [
		MatButtonModule,
		MatCardModule,
		MatIconModule,
		MatPaginatorModule,
		MatProgressSpinnerModule,
		MatSortModule,
		MatTableModule,
		RouterLink,
	],
	selector: 'app-employee-list',
	styleUrl: './employee-list.component.scss',
	templateUrl: './employee-list.component.html',
})
export class EmployeeListComponent {
	private readonly api = inject(EmployeesApiService);
	private readonly dialog = inject(MatDialog);
	private readonly notifications = inject(NotificationService);
	private readonly sort = viewChild(MatSort);
	private readonly paginator = viewChild(MatPaginator);

	protected readonly columns: readonly string[] = [
		'firstName',
		'lastName',
		'email',
		'phone',
		'city',
		'gender',
		'isActive',
	];

	protected readonly employees = rxResource({ stream: () => this.api.getAll() });
	protected readonly dataSource = new MatTableDataSource<Employee>();
	protected readonly trackById: TrackByFunction<Employee> = (_, employee) => employee.id;

	constructor() {
		effect(() => {
			this.dataSource.data = this.employees.hasValue() ? this.employees.value() : [];
		});
		effect(() => {
			this.dataSource.sort = this.sort();
			this.dataSource.paginator = this.paginator();
		});
	}

	protected async edit(employee: Employee): Promise<void> {
		const dialogRef = this.dialog.open<EmployeeEditDialogComponent, Employee, Employee>(
			EmployeeEditDialogComponent,
			{ data: employee, panelClass: 'employee-edit-dialog' },
		);
		const updated = await firstValueFrom(dialogRef.afterClosed());

		if (updated) {
			this.employees.reload();
			this.notifications.success('Changes saved.');
		} else {
			this.notifications.info('Editing cancelled.');
		}
	}
}
