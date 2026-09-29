package main

import (
	"context"
	"flag"
	"fmt"
	"fuel-price-pipeline/adapters"
	"fuel-price-pipeline/ports"
	"fuel-price-pipeline/service"
	"log"
	"net/http"
	"os"
	"strings"
	"time"

	"github.com/go-chi/chi/v5"
	"github.com/go-chi/cors"
)

func main() {

	// -serve-only skips the startup pipeline run (EIA download, DB save,
	// CSV export) and goes straight to serving the API. Useful for local
	// testing when you don't want to hit EIA on every restart.
	serveOnly := flag.Bool("serve-only", false, "skip the startup EIA download/save/CSV export; just serve the API")
	flag.Parse()

	// Configuration (from environment; see .env.example)
	apiKey := strings.TrimSpace(os.Getenv("EIA_API_KEY"))
	if apiKey == "" {
		log.Fatal("EIA_API_KEY environment variable not set. Exiting now...")
	}
	connString := strings.TrimSpace(os.Getenv("FUEL_DSN"))
	if connString == "" {
		log.Fatal("FUEL_DSN environment variable not set. Exiting now...")
	}
	csvFilename := "diesel_fuel_prices.csv"

	postgresRepo, err := adapters.NewPostgresRepository(connString)
	if err != nil {
		log.Fatal("Failed to connect to database:", err)
	}

	fuelService := service.NewFuelService(postgresRepo, apiKey)
	server := ports.NewHttpServer(fuelService)
	ctx := context.Background()

	// Run the pipeline once on startup: fetch from EIA, persist, export CSV.
	// Skipped with -serve-only.
	if *serveOnly {
		fmt.Println("Serve-only mode: skipping EIA download, DB save, and CSV export")
	} else {
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
	}

	// Setup Chi router and serve the API.
	r := chi.NewRouter()

	// CORS: allow the Angular dev server (override with CORS_ORIGIN).
	corsOrigin := strings.TrimSpace(os.Getenv("CORS_ORIGIN"))
	if corsOrigin == "" {
		corsOrigin = "http://localhost:4200"
	}
	r.Use(cors.Handler(cors.Options{
		AllowedOrigins: []string{corsOrigin},
		AllowedMethods: []string{"GET", "POST", "OPTIONS"},
		AllowedHeaders: []string{"Content-Type"},
	}))

	// Health check: confirms the server is up and the DB is reachable,
	// without touching EIA or any tables.
	r.Get("/healthz", func(w http.ResponseWriter, r *http.Request) {
		if err := postgresRepo.Pool.Ping(r.Context()); err != nil {
			http.Error(w, "db unreachable: "+err.Error(), http.StatusServiceUnavailable)
			return
		}
		w.WriteHeader(http.StatusOK)
		fmt.Fprintln(w, "ok")
	})

	r.Get("/getEIAData", server.GetEIADataHandler)
	r.Get("/getAll", server.GetAllHandler)
	r.Post("/save", server.SaveHandler)

	addr := strings.TrimSpace(os.Getenv("API_ADDR"))
	if addr == "" {
		addr = ":8080"
	}

	srv := &http.Server{
		Addr:              addr,
		Handler:           r,
		ReadHeaderTimeout: 5 * time.Second,
		ReadTimeout:       10 * time.Second,
		WriteTimeout:      15 * time.Second,
		IdleTimeout:       60 * time.Second,
	}

	fmt.Printf("Serving API on %s\n", addr)
	if err := srv.ListenAndServe(); err != nil {
		log.Fatal("Server failed:", err)
	}

}
