package adapters

import (
	"context"
	"fmt"
	"fuel-downloader/domain"
	"log"

	"github.com/jackc/pgx/v5/pgxpool"
)

type FuelRate = domain.FuelRate
type EIAResponse = domain.EIAResponse

type PostgresRepository struct {
	Pool *pgxpool.Pool
}

func NewPostgresRepository(connString string) (*PostgresRepository, error) {
	pool, err := pgxpool.New(context.Background(), connString)
	if err != nil {
		return nil, fmt.Errorf("unable to create connection pool: %w", err)
	}

	// Test the connection
	err = pool.Ping(context.Background())
	if err != nil {
		return nil, fmt.Errorf("unable to ping database: %w", err)
	}

	log.Println("Database connected successfully")

	return &PostgresRepository{Pool: pool}, nil
}

func (r *PostgresRepository) GetAll(ctx context.Context) ([]FuelRate, error) {
	sqlQuery := `
		SELECT product_code, area_code, period, value, unit, product_name, area_name, created_at
		FROM eia.fuel_rate
		ORDER BY period DESC
	`

	rows, err := r.Pool.Query(ctx, sqlQuery)
	if err != nil {
		return nil, fmt.Errorf("failed to query fuel rates: %w", err)
	}
	defer rows.Close()

	var fuelRates []FuelRate
	for rows.Next() {
		var fr FuelRate
		err := rows.Scan(&fr.Product, &fr.AreaCode, &fr.Period, &fr.Value,
			&fr.Units, &fr.ProductName, &fr.AreaName, &fr.CreatedAt)
		if err != nil {
			return nil, fmt.Errorf("failed to scan row: %w", err)
		}
		fuelRates = append(fuelRates, fr)
	}

	return fuelRates, nil
}

func (r *PostgresRepository) Save(ctx context.Context, fuelRates []domain.FuelRate) error {
	sqlQuery := `
		INSERT INTO eia.fuel_rate 
		(product_code, area_code, period, value, unit, product_name, area_name, raw)
		VALUES ($1, $2, $3, $4, $5, $6, $7, '{}'::jsonb)
		ON CONFLICT (product_code, area_code, period)
		DO UPDATE SET
			value = EXCLUDED.value,
			unit = EXCLUDED.unit,
			product_name = EXCLUDED.product_name,
			area_name = EXCLUDED.area_name,
			updated_at = NOW()
	`

	for _, fr := range fuelRates {
		_, err := r.Pool.Exec(ctx, sqlQuery,
			fr.Product, fr.AreaCode, fr.Period, fr.Value, fr.Units,
			fr.ProductName, fr.AreaName)
		if err != nil {
			return fmt.Errorf("failed to insert fuel rate: %w", err)
		}
	}

	return nil
}
