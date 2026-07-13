import { Component, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GetLatestService } from '../../usecase/get-latest.service';

@Component({
  selector: 'app-fuel-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './fuel-table.component.html',
  styleUrls: ['./fuel-table.component.css']
})
export class FuelTableComponent implements OnInit {
  fuelRate = computed(() => this.getLatest.latestRate());

  constructor(private getLatest: GetLatestService) {}

  ngOnInit(): void {
    this.getLatest.refresh();
  }

  refresh(): void {
    this.getLatest.refresh();
  }
}