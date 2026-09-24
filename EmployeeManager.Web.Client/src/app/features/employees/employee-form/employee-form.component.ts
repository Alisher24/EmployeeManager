import { Component, computed, inject, input, OnInit, output, viewChild } from '@angular/core';
import {
	AbstractControl,
	FormGroupDirective,
	NonNullableFormBuilder,
	ReactiveFormsModule,
	ValidationErrors,
	Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { DateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import {
	MAT_FORM_FIELD_DEFAULT_OPTIONS,
	MatFormFieldDefaultOptions,
} from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { formatDateOnly, parseDateOnly } from '../../../shared/date-only';
import { EMPLOYEE_CONSTRAINTS as limits } from '../data/employee.constraints';
import { EmployeeRequest, Gender } from '../data/employee.model';

const PHONE_PATTERN = /^\+?(?:[ ()-]*\d){7,15}[ ()-]*$/;

const ERROR_MESSAGES: Record<string, (error: ValidationErrors) => string> = {
	matDatepickerParse: () => 'Enter a valid date.',
	required: () => 'This field is required.',
	maxlength: ({ requiredLength }) => `Must be at most ${requiredLength} characters.`,
	email: () => 'Enter a valid email address.',
	phone: () => 'Enter a valid phone number.',
	matDatepickerMax: () => `Employee must be at least ${limits.minAge} years old.`,
	matDatepickerMin: () => `Date of birth can't be more than ${limits.maxAge} years ago.`,
};

function phoneValidator(control: AbstractControl<string>): ValidationErrors | null {
	return !control.value || PHONE_PATTERN.test(control.value) ? null : { phone: true };
}

@Component({
	imports: [
		MatButtonModule,
		MatButtonToggleModule,
		MatDatepickerModule,
		MatInputModule,
		MatRadioModule,
		ReactiveFormsModule,
	],
	providers: [
		{
			provide: MAT_FORM_FIELD_DEFAULT_OPTIONS,
			useValue: { appearance: 'outline' } satisfies MatFormFieldDefaultOptions,
		},
	],
	selector: 'app-employee-form',
	styleUrl: './employee-form.component.scss',
	templateUrl: './employee-form.component.html',
})
export class EmployeeFormComponent implements OnInit {
	private readonly fb = inject(NonNullableFormBuilder);
	private readonly dateAdapter = inject<DateAdapter<Date>>(DateAdapter);
	private readonly formDirective = viewChild.required(FormGroupDirective);

	readonly employee = input<EmployeeRequest>();
	readonly saving = input(false);

	readonly saved = output<EmployeeRequest>();
	readonly cancelled = output();

	protected readonly form = this.fb.group({
		firstName: ['', [Validators.required, Validators.maxLength(limits.nameMaxLength)]],
		lastName: ['', [Validators.required, Validators.maxLength(limits.nameMaxLength)]],
		dateOfBirth: this.fb.control<Date | null>(null, Validators.required),
		gender: this.fb.control<Gender | null>(null, Validators.required),
		address1: ['', [Validators.required, Validators.maxLength(limits.addressMaxLength)]],
		address2: ['', Validators.maxLength(limits.addressMaxLength)],
		city: ['', [Validators.required, Validators.maxLength(limits.cityMaxLength)]],
		postalCode: ['', [Validators.required, Validators.maxLength(limits.postalCodeMaxLength)]],
		country: ['', [Validators.required, Validators.maxLength(limits.countryMaxLength)]],
		email: [
			'',
			[Validators.required, Validators.email, Validators.maxLength(limits.emailMaxLength)],
		],
		phone: [
			'',
			[Validators.required, phoneValidator, Validators.maxLength(limits.phoneMaxLength)],
		],
		isActive: [true],
	});

	protected readonly minDateOfBirth = this.yearsAgo(limits.maxAge);
	protected readonly maxDateOfBirth = this.yearsAgo(limits.minAge);

	private readonly initialValue = computed(() => {
		const employee = this.employee();

		return (
			employee && {
				...employee,
				dateOfBirth: parseDateOnly(employee.dateOfBirth),
				address2: employee.address2 ?? '',
			}
		);
	});

	ngOnInit(): void {
		this.form.reset(this.initialValue());
	}
	reset(): void {
		this.formDirective().resetForm(this.initialValue());
	}

	protected submit(): void {
		const { dateOfBirth, gender, address2, ...value } = this.form.getRawValue();

		if (this.form.invalid || !dateOfBirth || !gender) {
			this.form.markAllAsTouched();
			return;
		}

		this.saved.emit({
			...value,
			dateOfBirth: formatDateOnly(dateOfBirth),
			gender,
			address2: address2 || null,
		});
	}

	protected errorMessage(control: AbstractControl): string {
		const errors = control.errors ?? {};
		const key = Object.keys(ERROR_MESSAGES).find((name) => name in errors);

		return key ? ERROR_MESSAGES[key](errors[key]) : '';
	}

	private yearsAgo(years: number): Date {
		return this.dateAdapter.addCalendarYears(this.dateAdapter.today(), -years);
	}
}
