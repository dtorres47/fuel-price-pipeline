package adapters

import (
	"context"
	"encoding/csv"
	"fmt"
	"fuel-price-pipeline/domain"
	"os"
)

// ExportToCSV queries all fuel rates and writes them to a CSV file
func ExportToCSV(r *PostgresRepository, filename string) error {
	// TODO: implement locks for concurrent writes

	// Query all fuel rates from database
	sqlQuery := `
		SELECT product_code, area_code, period, value, unit, product_name, area_name, created_at
		FROM fuel_price.diesel_fuel_price
		ORDER BY period DESC
	`

	rows, err := r.Pool.Query(context.Background(), sqlQuery)
	if err != nil {
		return fmt.Errorf("failed to query fuel rates: %w", err)
	}
	defer rows.Close()

	// Create CSV file
	file, err := os.Create(filename)
	if err != nil {
		return fmt.Errorf("failed to create CSV file: %w", err)
	}
	defer file.Close()

	// Create CSV writer
	writer := csv.NewWriter(file)
	defer writer.Flush()

	// Write header row
	header := []string{"Product Code", "Product Name", "Area Code", "Area Name", "Period", "Value", "Units", "Created At"}
	if err := writer.Write(header); err != nil {
		return fmt.Errorf("failed to write CSV header: %w", err)
	}

	// Write data rows
	for rows.Next() {
		var fr domain.DieselFuelPrice
		err := rows.Scan(&fr.ProductCode, &fr.AreaCode, &fr.Period, &fr.Value,
			&fr.Unit, &fr.ProductName, &fr.AreaName, &fr.CreatedAt)
		if err != nil {
			return fmt.Errorf("failed to scan row: %w", err)
		}

		// Convert to string slice for CSV
		record := []string{
			fr.ProductCode,
			fr.ProductName,
			fr.AreaCode,
			fr.AreaName,
			fr.Period.Format("2006-01-02"),
			fmt.Sprintf("%.4f", fr.Value),
			fr.Unit,
			fr.CreatedAt.Format("2006-01-02 15:04:05"),
		}

		if err := writer.Write(record); err != nil {
			return fmt.Errorf("failed to write CSV row: %w", err)
		}
	}

	// Check for errors during iteration
	if err = rows.Err(); err != nil {
		return fmt.Errorf("error iterating rows: %w", err)
	}

	return nil
}
