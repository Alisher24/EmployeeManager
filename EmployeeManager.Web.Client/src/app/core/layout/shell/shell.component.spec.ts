import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { ShellComponent } from './shell.component';

describe('ShellComponent', () => {
	let fixture: ComponentFixture<ShellComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [ShellComponent],
			providers: [provideRouter([])],
		}).compileComponents();

		fixture = TestBed.createComponent(ShellComponent);
		await fixture.whenStable();
	});

	it('should link to every screen', () => {
		const links = Array.from<HTMLAnchorElement>(
			fixture.nativeElement.querySelectorAll('a[mat-list-item]'),
		);

		expect(links.map((link) => link.getAttribute('href'))).toEqual([
			'/dashboard',
			'/employees/new',
			'/employees',
		]);
	});

	it('should toggle the menu from the toolbar', async () => {
		const toggle: HTMLButtonElement = fixture.nativeElement.querySelector(
			'button[aria-label="Toggle menu"]',
		);

		expect(toggle.getAttribute('aria-expanded')).toBe('true');

		toggle.click();
		await fixture.whenStable();

		expect(toggle.getAttribute('aria-expanded')).toBe('false');

		toggle.click();
		await fixture.whenStable();

		expect(toggle.getAttribute('aria-expanded')).toBe('true');
	});
});
