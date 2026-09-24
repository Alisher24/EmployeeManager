import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
	MAT_DIALOG_DATA,
	MatDialogContent,
	MatDialogRef,
	MatDialogTitle,
} from '@angular/material/dialog';
import { filter, firstValueFrom } from 'rxjs';
import { Employee, EmployeeRequest } from '../data/employee.model';
import { EmployeesApiService } from '../data/employees-api.service';
import { EmployeeFormComponent } from '../employee-form/employee-form.component';

@Component({
	imports: [EmployeeFormComponent, MatDialogContent, MatDialogTitle],
	selector: 'app-employee-edit-dialog',
	styleUrl: './employee-edit-dialog.component.scss',
	templateUrl: './employee-edit-dialog.component.html',
})
export class EmployeeEditDialogComponent {
	private readonly api = inject(EmployeesApiService);
	private readonly dialogRef =
		inject<MatDialogRef<EmployeeEditDialogComponent, Employee>>(MatDialogRef);

	protected readonly employee = inject<Employee>(MAT_DIALOG_DATA);
	protected readonly saving = signal(false);

	constructor() {
		this.dialogRef.disableClose = true;
		this.dialogRef
			.keydownEvents()
			.pipe(
				filter((event) => event.key === 'Escape' && !this.saving()),
				takeUntilDestroyed(),
			)
			.subscribe(() => this.cancel());
	}

	protected async save(request: EmployeeRequest): Promise<void> {
		this.saving.set(true);

		try {
			const updated = await firstValueFrom(this.api.update(this.employee.id, request));
			this.dialogRef.close(updated);
		} catch {

		} finally {
			this.saving.set(false);
		}
	}

	protected cancel(): void {
		this.dialogRef.close();
	}
}
