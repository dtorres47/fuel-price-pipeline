package ports

import (
	"encoding/json"
	"fuel-price-pipeline/domain"
	"fuel-price-pipeline/service"
	"io"
	"log"
	"net/http"
	"sync"
)

// HttpServer http fuel service
// NOTE: fuel service uses delegates for the repo.
type HttpServer struct {
	fuelService *service.FuelService // postgres repo inherits
	lock        sync.RWMutex
}

func NewHttpServer(fuelService *service.FuelService) *HttpServer {
	return &HttpServer{
		fuelService: fuelService,
	}
}

// GetEIADataHandler fetches fresh data from EIA API
func (s *HttpServer) GetEIADataHandler(w http.ResponseWriter, r *http.Request) {
	fuelRates, err := s.fuelService.GetFromEIA()
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(fuelRates)
}

// GetAllHandler returns all fuel rates from the database
func (s *HttpServer) GetAllHandler(w http.ResponseWriter, r *http.Request) {
	fuelRates, err := s.fuelService.GetAll(r.Context())
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(fuelRates)
}

// SaveHandler saves fuel rates to the database
func (s *HttpServer) SaveHandler(w http.ResponseWriter, r *http.Request) {
	s.lock.Lock()
	defer s.lock.Unlock()

	body, err := io.ReadAll(r.Body)
	if err != nil {
		http.Error(w, "could not read body", http.StatusInternalServerError)
		return
	}

	defer r.Body.Close()

	defer func(Body io.ReadCloser) {
		err := Body.Close()
		if err != nil {
			log.Println("Failed to close body:", err)
		}
	}(r.Body)

	var fuelRates []domain.DieselFuelPrice

	if err := json.Unmarshal(body, &fuelRates); err != nil {
		log.Println("Failed to unmarshal payload:", err)
		w.WriteHeader(http.StatusBadRequest)
		http.Error(w, "could not unmarshal payload", http.StatusInternalServerError)
		return
	}

	if err := s.fuelService.Save(r.Context(), fuelRates); err != nil {
		http.Error(w, "failed to save fuel rates", http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusCreated)
	json.NewEncoder(w).Encode(map[string]string{"status": "saved"})
}
