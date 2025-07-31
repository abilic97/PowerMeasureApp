import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { EChartsOption } from 'echarts';
import { environment } from 'src/environments/environment';
import { AuthService } from '../service/auth.service';

interface NoOfPowerCuts {
  count: number,
  date: number
}

@Component({
  selector: 'app-noof-power-cuts',
  templateUrl: './noof-power-cuts.component.html',
  styleUrls: ['./noof-power-cuts.component.css']
})
export class NoofPowerCutsComponent implements OnInit {

  theme: any;
  date = new Date();
  apidata: NoOfPowerCuts[];
  chartOption: EChartsOption;
  finalApi: string;
  @Input() userId: any;
  constructor(private service: AuthService, private http: HttpClient) { }

  ngOnInit(): void {
    this.theme = environment.graphTheme;
    this.getNoofPowerCursPerMonth();
  }

  toMonthName(monthNumber: any) {
    const date = new Date();
    date.setMonth(monthNumber - 1);

    return date.toLocaleString('en-US', {
      month: 'long',
    });
  }
  getNoofPowerCursPerMonth() {
    let consumerApi = 'https://localhost:5001/api/consumption/noofPo/';
    if(this.userId == null) {
      let id = this.service.getUserIdFromToken();
      this.finalApi = consumerApi + id;
    }
    else {
      this.finalApi = consumerApi + this.userId;
    }
    this.http.get(this.finalApi).subscribe(response => {
      console.log(response);
      this.apidata = response as NoOfPowerCuts[];
      this.initializeGraph();

    }, err => {
      console.log(err);
    }
    );
  }

  initializeGraph() {
    this.chartOption = {
      tooltip: {},

      radius: '100%',
      xAxis: {
        data: this.apidata.map(x => this.toMonthName(x.date)),
      },
      yAxis: {
        type: 'value',
      },
      series: [
        {
          data: this.apidata.map(x => x.count),
          type: 'effectScatter',
        },
      ],
    };
  }
}
