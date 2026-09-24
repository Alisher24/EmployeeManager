import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { map } from 'rxjs';

interface NavItem {
	readonly path: string;
	readonly label: string;
	readonly icon: string;
}

const MOBILE_QUERIES = [Breakpoints.XSmall, Breakpoints.Small];

@Component({
	imports: [
		MatButtonModule,
		MatIconModule,
		MatListModule,
		MatSidenavModule,
		MatToolbarModule,
		RouterLink,
		RouterLinkActive,
		RouterOutlet,
	],
	selector: 'app-shell',
	styleUrl: './shell.component.scss',
	templateUrl: './shell.component.html',
})
export class ShellComponent {
	private readonly breakpointObserver = inject(BreakpointObserver);

	protected readonly navItems: readonly NavItem[] = [
		{ path: '/dashboard', label: 'Dashboard', icon: 'dashboard' },
		{ path: '/employees/new', label: 'Create Employee', icon: 'person_add' },
		{ path: '/employees', label: 'View Employees', icon: 'group' },
	];

	protected readonly isMobile = toSignal(
		this.breakpointObserver.observe(MOBILE_QUERIES).pipe(map((state) => state.matches)),
		{ initialValue: this.breakpointObserver.isMatched(MOBILE_QUERIES) },
	);

	protected closeIfMobile(sidenav: MatSidenav): void {
		if (this.isMobile()) {
			void sidenav.close();
		}
	}
}
