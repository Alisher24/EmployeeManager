import { Component } from '@angular/core';
import { ShellComponent } from './core/layout/shell/shell.component';

@Component({
	imports: [ShellComponent],
	selector: 'app-root',
	templateUrl: './app.component.html',
})
export class AppComponent {}
