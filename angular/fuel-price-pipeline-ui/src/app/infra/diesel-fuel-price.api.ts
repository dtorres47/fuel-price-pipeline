// Raw shape of the Go API response (snake_case, as serialized by the
// Go DieselFuelPrice struct). Kept separate from the domain model so the
// wire format never leaks past the infra boundary.
export interface DieselFuelPriceApi {
  id?: number;
  product_code: string;
  area_code: string;
  period: string;
  value: number;
  unit: string;
  product_name: string;
  area_name: string;
  created_at?: string;
  updated_at?: string;
}
