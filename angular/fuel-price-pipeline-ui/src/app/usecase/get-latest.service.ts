import { Injectable, signal } from '@angular/core';
import { FuelRate } from '../domain/fuel-rate';
import { FuelService } from '../infra/fuel.service';

@Injectable({
  providedIn: 'root'
})
export class GetLatestService {
  latestRate = signal<FuelRate | null>(null);

  constructor(private fuelService: FuelService) {}

  refresh(area: string = 'NUS'): void {
    this.fuelService.getLatest(area).subscribe({
      next: (rate) => this.latestRate.set(rate),
      error: (err) => console.error('Failed to fetch fuel rate:', err)
    });
  }
}