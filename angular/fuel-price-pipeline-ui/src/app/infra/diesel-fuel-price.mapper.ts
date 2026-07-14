import { DieselFuelPrice } from '../domain/diesel-fuel-price';
import { DieselFuelPriceApi } from './diesel-fuel-price.api';

// Translates the Go API's snake_case payload into the domain model's
// camelCase shape. Mirrors the EIAResponse -> DieselFuelPrice mapping on
// the Go side: raw names stay at the boundary, clean names inside the app.
export function toDieselFuelPrice(raw: DieselFuelPriceApi): DieselFuelPrice {
  return {
    productCode: raw.product_code,
    productName: raw.product_name,
    areaCode: raw.area_code,
    areaName: raw.area_name,
    period: raw.period,
    value: raw.value,
    unit: raw.unit,
  };
}
