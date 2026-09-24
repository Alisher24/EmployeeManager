import { HarnessLoader, parallel } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatButtonToggleHarness } from '@angular/material/button-toggle/testing';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatErrorHarness, MatFormFieldHarness } from '@angular/material/form-field/testing';
import { MatInputHarness } from '@angular/material/input/testing';
import { MatRadioButtonHarness } from '@angular/material/radio/testing';
import { EmployeeRequest } from '../data/employee.model';
import { EmployeeFormComponent } from './employee-form.component';

const employee: EmployeeRequest = {
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

describe('EmployeeFormComponent', () => {
	const saved = vi.fn();
	const cancelled = vi.fn();
	let fixture: ComponentFixture<EmployeeFormComponent>;
	let loader: HarnessLoader;

	beforeEach(async () => {
		saved.mockClear();
		cancelled.mockClear();

		await TestBed.configureTestingModule({
			imports: [EmployeeFormComponent],
			providers: [provideNativeDateAdapter()],
		}).compileComponents();

		fixture = TestBed.createComponent(EmployeeFormComponent);
		fixture.componentInstance.saved.subscribe(saved);
		fixture.componentInstance.cancelled.subscribe(cancelled);
		loader = TestbedHarnessEnvironment.loader(fixture);
	});

	async function setValues(values: Record<string, string>): Promise<void> {
		for (const [label, value] of Object.entries(values)) {
			const input = await loader.getHarness(MatInputHarness.with({ label }));
			await input.setValue(value);
		}
	}

	async function getValue(label: string): Promise<string> {
		const input = await loader.getHarness(MatInputHarness.with({ label }));
		return input.getValue();
	}

	async function getErrors(label: string): Promise<string[]> {
		const field = await loader.getHarness(
			MatFormFieldHarness.with({ floatingLabelText: label }),
		);
		return field.getTextErrors();
	}

	async function getAllErrors(): Promise<string[]> {
		const errors = await loader.getAllHarnesses(MatErrorHarness);
		return parallel(() => errors.map((error) => error.getText()));
	}

	async function clickButton(text: string): Promise<void> {
		const button = await loader.getHarness(MatButtonHarness.with({ text }));
		await button.click();
	}

	it('should emit the entered employee with the date of birth as yyyy-MM-dd', async () => {
		await setValues({
			'First name': 'Jane',
			'Last name': 'Doe',
			'Date of birth': '5/17/1990',
			'Address line 1': '221B Baker Street',
			City: 'London',
			'Postal code': 'NW1 6XE',
			Country: 'United Kingdom',
			Email: 'jane.doe@example.com',
			Mobile: '+447700900123',
		});
		const female = await loader.getHarness(MatButtonToggleHarness.with({ text: 'Female' }));
		await female.check();

		await clickButton('Save');

		expect(saved).toHaveBeenCalledExactlyOnceWith(employee);
	});

	it('should show the errors instead of emitting an invalid form', async () => {
		await clickButton('Save');

		expect(saved).not.toHaveBeenCalled();
		expect(await getErrors('First name')).toEqual(['This field is required.']);
		expect(await getErrors('Address line 2')).toEqual([]);
		expect(await getAllErrors()).toHaveLength(10);
	});

	it('should explain the invalid values', async () => {
		await setValues({ 'Date of birth': 'not a date', Email: 'jane.doe', Mobile: '12-34' });

		await clickButton('Save');

		expect(await getErrors('Date of birth')).toEqual(['Enter a valid date.']);
		expect(await getErrors('Email')).toEqual(['Enter a valid email address.']);
		expect(await getErrors('Mobile')).toEqual(['Enter a valid phone number.']);
	});

	it('should not accept an employee younger than 16', async () => {
		const today = new Date();
		const fifteenYearsAgo = `${today.getMonth() + 1}/${today.getDate()}/${today.getFullYear() - 15}`;

		await setValues({ 'Date of birth': fifteenYearsAgo });
		await clickButton('Save');

		const errors = await getErrors('Date of birth');
		expect(errors).toEqual(['Employee must be at least 16 years old.']);
	});

	it('should fill in the employee and emit it back unchanged', async () => {
		fixture.componentRef.setInput('employee', employee);

		const gender = await loader.getHarness(MatButtonToggleHarness.with({ checked: true }));
		const status = await loader.getHarness(MatRadioButtonHarness.with({ checked: true }));

		expect(await getValue('First name')).toBe('Jane');
		expect(await getValue('Date of birth')).toBe('5/17/1990');
		expect(await gender.getText()).toBe('Female');
		expect(await status.getLabelText()).toBe('Active');

		await clickButton('Save');

		expect(saved).toHaveBeenCalledExactlyOnceWith(employee);
	});

	it('should reset to the employee', async () => {
		fixture.componentRef.setInput('employee', employee);
		await setValues({ 'First name': 'John' });

		fixture.componentInstance.reset();

		expect(await getValue('First name')).toBe('Jane');
	});

	it('should not show errors after a reset', async () => {
		await clickButton('Save');

		expect(await getAllErrors()).toHaveLength(10);

		fixture.componentInstance.reset();

		expect(await getAllErrors()).toEqual([]);
	});

	it('should emit cancelled on Cancel', async () => {
		await clickButton('Cancel');

		expect(cancelled).toHaveBeenCalledOnce();
		expect(saved).not.toHaveBeenCalled();
	});

	it('should disable the buttons while saving', async () => {
		fixture.componentRef.setInput('saving', true);

		const save = await loader.getHarness(MatButtonHarness.with({ text: 'Save' }));
		const cancel = await loader.getHarness(MatButtonHarness.with({ text: 'Cancel' }));

		expect(await save.isDisabled()).toBe(true);
		expect(await cancel.isDisabled()).toBe(true);
	});
});
