import { Component, OnInit, Input } from '@angular/core';
import { EChartsOption } from 'echarts';
import { environment } from 'src/environments/environment';
import { Monthlyconsumption } from '../monthlyconsumption';

@Component({
  selector: 'app-totalcost',
  templateUrl: './totalcost.component.html',
  styleUrls: ['./totalcost.component.css']

})
export class TotalcostComponent implements OnInit {
  theme: any;
  chartOption: EChartsOption;
  @Input() data: Monthlyconsumption[];

  constructor() { }

  ngOnInit(): void {
    this.theme = environment.graphTheme;
    this.initializeGraph();
  }

  toMonthName(monthNumber: any) {
    const date = new Date();
    date.setMonth(monthNumber - 1);

    return date.toLocaleString('en-US', {
      month: 'long',
    });
  }

  initializeGraph() {
    this.chartOption = {
      tooltip: {},

      radius: '100%',
      xAxis: {
        data: this.data.map(x => this.toMonthName(x.month)),
      },
      yAxis: {
        type: 'value',
      },
      series: [
        {
          data: this.data.map(x => x.cost),
          type: 'line',
        },
      ],
    };
  }
}
