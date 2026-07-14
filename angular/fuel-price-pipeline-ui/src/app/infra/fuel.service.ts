import { Injectable } from '@angular/core';
import { of, delay, Observable } from 'rxjs';
import { DieselFuelPrice } from '../domain/diesel-fuel-price';

@Injectable({
  providedIn: 'root'
})
export class FuelService {
  getLatest(area: string = 'NUS'): Observable<DieselFuelPrice> {
    const mock: DieselFuelPrice = {
      productCode: 'EPD2D',
      productName: 'No 2 Diesel',
      areaCode: 'NUS',
      areaName: 'U.S.',
      period: '2025-08',
      value: 3.744,
      unit: '$/GAL'
    };

    // Simulate network latency with delay
    return of(mock).pipe(delay(1000));
  }
}