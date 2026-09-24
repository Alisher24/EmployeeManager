import { Routes } from '@angular/router';

export const routes: Routes = [
	{ path: '', pathMatch: 'full', redirectTo: 'dashboard' },
	{
		path: 'dashboard',
		title: 'Dashboard',
		loadComponent: () =>
			import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent),
	},
	{
		path: 'employees/new',
		title: 'Create Employee',
		loadComponent: () =>
			import('./features/employees/employee-create/employee-create.component').then(
				(m) => m.EmployeeCreateComponent,
			),
	},
	{
		path: 'employees',
		title: 'View Employees',
		loadComponent: () =>
			import('./features/employees/employee-list/employee-list.component').then(
				(m) => m.EmployeeListComponent,
			),
	},
	{ path: '**', redirectTo: 'dashboard' },
];
