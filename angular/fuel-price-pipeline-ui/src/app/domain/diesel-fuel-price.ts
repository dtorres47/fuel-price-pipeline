// Domain model (idiomatic camelCase).
// NOTE: the Go API serializes snake_case (product_code, area_code, unit,
// product_name, area_name). When fuel.service.ts is wired to the real
// endpoint, map the raw response into this shape at the infra boundary
// (same pattern as EIAResponse -> DieselFuelPrice on the Go side).
export interface DieselFuelPrice {
    productCode: string;
    productName: string;
    areaCode: string;
    areaName: string;
    period: string;
    value: number;
    unit: string;
}
