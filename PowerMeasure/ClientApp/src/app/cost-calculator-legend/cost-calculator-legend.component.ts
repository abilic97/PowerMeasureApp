import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-cost-calculator-legend',
  templateUrl: './cost-calculator-legend.component.html',
  styleUrls: ['./cost-calculator-legend.component.css']
})
export class CostCalculatorLegendComponent implements OnInit {
  items = [
    {
      name: 'Energy',
      amount: '1 kWh',
      cost: 0.5295
    },
    {
      name: 'Transfer',
      amount: '1 kWh',
      cost: 0.0900
    },
    {
      name: 'Distribution',
      amount: '1 kWh',
      cost: 0.2200
    },
    {
      name: 'Supply',
      amount: '1 month',
      cost: 7.40
    },
    {
      name: 'Measuring location',
      amount: '1 month',
      cost: 11.60
    },
    {
      name: 'RES',
      amount: '1 kWh',
      cost: 0.1050
    },
  ]

  constructor() { }

  ngOnInit(): void {
  }

}
