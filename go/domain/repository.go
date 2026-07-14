package domain

import (
	"context"
)

type Repository interface {
	GetAll(ctx context.Context) ([]DieselFuelPrice, error)
	Save(ctx context.Context, fuelRates []DieselFuelPrice) error
}
