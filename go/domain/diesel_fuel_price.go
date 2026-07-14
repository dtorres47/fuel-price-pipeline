package domain

import (
	"time"
)

type DieselFuelPrice struct {
	ID          int       `json:"id"`
	ProductCode string    `json:"product_code"`
	AreaCode    string    `json:"area_code"`
	Period      time.Time `json:"period"`
	Value       float64   `json:"value"`
	Unit        string    `json:"unit"`
	ProductName string    `json:"product_name"`
	AreaName    string    `json:"area_name"`
	CreatedAt   time.Time `json:"created_at"`
	UpdatedAt   time.Time `json:"updated_at"`
}
