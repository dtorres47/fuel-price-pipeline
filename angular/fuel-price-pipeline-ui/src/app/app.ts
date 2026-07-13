import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FuelTableComponent } from './components/fuel-table/fuel-table.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, FuelTableComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('fuel-price-pipeline-ui');
}
