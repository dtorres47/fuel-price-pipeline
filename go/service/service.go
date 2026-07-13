package service

import (
	"context"
	"encoding/json"
	"fmt"
	"fuel-price-pipeline/domain"
	"io"
	"net/http"
	"strconv"
	"time"
)

// EIA API endpoint for diesel prices
// TODO: move this to config
var eiaUrl = "https://api.eia.gov/v2/petroleum/pri/gnd/data/?api_key=%s&frequency=weekly&data[0]=value&sort[0][column]=period&sort[0][direction]=desc&offset=0&length=10"

type FuelRate = domain.FuelRate
type EIAResponse = domain.EIAResponse

type FuelService struct {
	repo   domain.Repository
	apiKey string
}

// GetAll delegates to the repo.
func (s *FuelService) GetAll(ctx context.Context) ([]FuelRate, error) {
	return s.repo.GetAll(ctx)
}

// Save delegates to the repo.
func (s *FuelService) Save(ctx context.Context, fuelRates []FuelRate) error {
	return s.repo.Save(ctx, fuelRates)
}

func NewFuelService(repo domain.Repository, apiKey string) *FuelService {
	return &FuelService{
		repo:   repo,
		apiKey: apiKey,
	}
}

// GetFromEIA Gets diesel fuel prices from EIA API.
func (s *FuelService) GetFromEIA() ([]FuelRate, error) {

	url := fmt.Sprintf(eiaUrl, s.apiKey)

	// Make HTTP request
	resp, err := http.Get(url)
	if err != nil {
		return nil, fmt.Errorf("failed to fetch from EIA: %w", err)
	}
	defer resp.Body.Close()

	// Check status code
	if resp.StatusCode != http.StatusOK {
		return nil, fmt.Errorf("EIA API returned status %d", resp.StatusCode)
	}

	// Read response body
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, fmt.Errorf("failed to read response: %w", err)
	}

	// Parse JSON
	var eiaResp EIAResponse
	err = json.Unmarshal(body, &eiaResp)
	if err != nil {
		return nil, fmt.Errorf("failed to parse JSON: %w", err)
	}

	// Convert to FuelRate structs
	var fuelRates []FuelRate
	for _, item := range eiaResp.Response.Data {
		// Parse period string (format: "YYYY-MM-DD")
		period, err := time.Parse("2006-01-02", item.Period)
		if err != nil {
			continue // Skip invalid dates
		}

		// Parse value string to float64
		value, err := strconv.ParseFloat(item.Value, 64)
		if err != nil {
			continue // Skip invalid values
		}

		fuelRate := FuelRate{
			Product:     item.Product,
			ProductName: item.ProductName,
			AreaCode:    item.DuoArea,
			AreaName:    item.AreaName,
			Period:      period,
			Value:       value,
			Units:       item.Units,
		}
		fuelRates = append(fuelRates, fuelRate)
	}

	return fuelRates, nil
}
