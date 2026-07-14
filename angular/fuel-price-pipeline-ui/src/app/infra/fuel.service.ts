import { Injectable } from '@angular/core';
import { of, delay, map, Observable } from 'rxjs';
import { DieselFuelPrice } from '../domain/diesel-fuel-price';
import { DieselFuelPriceApi } from './diesel-fuel-price.api';
import { toDieselFuelPrice } from './diesel-fuel-price.mapper';

@Injectable({
  providedIn: 'root'
})
export class FuelService {
  // TODO: wire to the live Go API.
  //   private readonly baseUrl = 'http://localhost:8080';
  //   constructor(private http: HttpClient) {}
  //
  //   getLatest(area: string = 'NUS'): Observable<DieselFuelPrice> {
  //     // GET /getAll returns all rows, newest first (period DESC).
  //     return this.http.get<DieselFuelPriceApi[]>(`${this.baseUrl}/getAll`).pipe(
  //       map(rows => toDieselFuelPrice(rows[0]))
  //     );
  //   }
  // Requires provideHttpClient() in app.config.ts and CORS on the Go server.

  getLatest(area: string = 'NUS'): Observable<DieselFuelPrice> {
    // Mock payload in the SAME snake_case shape the Go API returns, run
    // through the real mapper — so the mapping is exercised now and the
    // swap to HTTP later only replaces the data source, not the shape.
    const raw: DieselFuelPriceApi = {
      product_code: 'EPD2D',
      product_name: 'No 2 Diesel',
      area_code: 'NUS',
      area_name: 'U.S.',
      period: '2025-08',
      value: 3.744,
      unit: '$/GAL'
    };

    return of(raw).pipe(
      delay(1000),                       // simulate network latency
      map(toDieselFuelPrice)             // same mapper the live call will use
    );
  }
}
