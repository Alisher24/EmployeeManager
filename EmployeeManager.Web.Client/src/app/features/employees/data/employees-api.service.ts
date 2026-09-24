import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Employee, EmployeeRequest, EmployeeStats } from './employee.model';

@Injectable({ providedIn: 'root' })
export class EmployeesApiService {
	private readonly http = inject(HttpClient);
	private readonly baseUrl = `${environment.apiUrl}/employees`;

	getAll(): Observable<Employee[]> {
		return this.http.get<Employee[]>(this.baseUrl);
	}

	getById(id: string): Observable<Employee> {
		return this.http.get<Employee>(`${this.baseUrl}/${id}`);
	}

	create(request: EmployeeRequest): Observable<Employee> {
		return this.http.post<Employee>(this.baseUrl, request);
	}

	update(id: string, request: EmployeeRequest): Observable<Employee> {
		return this.http.put<Employee>(`${this.baseUrl}/${id}`, request);
	}

	getStats(): Observable<EmployeeStats> {
		return this.http.get<EmployeeStats>(`${this.baseUrl}/stats`);
	}
}
