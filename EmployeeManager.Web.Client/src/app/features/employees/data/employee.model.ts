export type Gender = 'Male' | 'Female';

export interface Employee {
	readonly id: string;
	readonly firstName: string;
	readonly lastName: string;
	readonly dateOfBirth: string;
	readonly gender: Gender;
	readonly address1: string;
	readonly address2: string | null;
	readonly city: string;
	readonly postalCode: string;
	readonly country: string;
	readonly email: string;
	readonly phone: string;
	readonly isActive: boolean;
}

export type EmployeeRequest = Omit<Employee, 'id'>;

export interface EmployeeStats {
	readonly total: number;
	readonly active: number;
	readonly male: number;
	readonly female: number;
}
