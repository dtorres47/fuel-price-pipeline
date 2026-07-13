package main

import (
	"context"
	"fmt"
	"fuel-price-pipeline/adapters"
	"fuel-price-pipeline/ports"
	"fuel-price-pipeline/service"
	"log"
	"net/http"
	"os"
	"strings"

	"github.com/go-chi/chi/v5"
)

func main() {

	// Configuration
	// TODO: add this to a config
	apiKey := strings.TrimSpace(os.Getenv("EIA_API_KEY"))
	if apiKey == "" {
		log.Fatal("EIA_API_KEY environment variable not set. Exiting now...")
	}
	connString := "postgresql://postgres:postgres@localhost:5432/fuel_price"
	csvFilename := "diesel_fuel_prices.csv"

	postgresRepo, err := adapters.NewPostgresRepository(connString)
	if err != nil {
		log.Fatal("Failed to connect to database:", err)
	}

	fuelService := service.NewFuelService(postgresRepo, apiKey)
	server := ports.NewHttpServer(fuelService)
	ctx := context.Background()

	// Run the pipeline once on startup: fetch from EIA, persist, export CSV.
	fmt.Println("Downloading fuel rates from EIA API...")
	fuelRates, err := fuelService.GetFromEIA()
	if err != nil {
		log.Fatal("Failed to get fuel rates:", err)
	}
	fmt.Printf("Downloaded %d fuel rates\n", len(fuelRates))

	fmt.Println("Saving to database...")
	if err := postgresRepo.Save(ctx, fuelRates); err != nil {
		log.Fatal("Failed to save fuel rates:", err)
	}
	fmt.Println("Saved to database successfully")

	fmt.Println("Exporting to CSV...")
	if err := adapters.ExportToCSV(postgresRepo, csvFilename); err != nil {
		log.Fatal("Failed to export CSV:", err)
	}
	fmt.Printf("Exported to %s successfully\n", csvFilename)

	// Setup Chi router and serve the API.
	r := chi.NewRouter()
	r.Get("/getEIAData", server.GetEIADataHandler)
	r.Get("/getAll", server.GetAllHandler)
	r.Post("/save", server.SaveHandler)

	addr := ":8080"
	fmt.Printf("Serving API on %s\n", addr)
	if err := http.ListenAndServe(addr, r); err != nil {
		log.Fatal("Server failed:", err)
	}
}
