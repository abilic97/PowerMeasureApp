import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { EChartsOption } from 'echarts';
import { environment } from 'src/environments/environment';
import { AuthService } from '../service/auth.service';

interface Energy {
  id: number
  current: number,
  voltage: number,
  power: number,
  reportDate: Date
}

@Component({
  selector: 'app-daily-all-individual-values',
  templateUrl: './daily-all-individual-values.component.html',
  styleUrls: ['./daily-all-individual-values.component.css']
})
export class DailyAllIndividualValuesComponent implements OnInit {
  @Input() date: FormControl;
  @Input() userId: any;
  theme: any;
  chartOptionV: EChartsOption;
  chartOptionC: EChartsOption;
  chartOptionP: EChartsOption;
  apiData: Energy[];
  finalApi: string;
  chartP: EChartsOption;
  chartV: EChartsOption;
  chartC: EChartsOption;

  constructor(private service: AuthService, private http: HttpClient) { }

  ngOnInit(): void {
    const currentDate = new Date();
    currentDate.setHours(0, 0, 0);
    this.getDailyPowerStatAll(currentDate);
    this.date.valueChanges.subscribe(val => {
      console.log(val, "here");
      if (typeof val != "undefined") {
        let selectedDate = new Date(val!.year, val!.month - 1, val!.day, 0, 0, 0);
        console.log(selectedDate, "test");
        this.getDailyPowerStatAll(selectedDate);
      }
    });

    this.setTheme();
  }

  setTheme() {
    this.theme = environment.graphTheme;
  }
  getDailyPowerStatAll(date: Date) {
    let apiUrl = 'https://localhost:5001/api/consumption/get-all-individual-values/';
    if (this.userId == null) {
      let id = this.service.getUserIdFromToken();
      this.finalApi = apiUrl + id;
    }
    else {
      this.finalApi = apiUrl + this.userId;
    }

    let params = new HttpParams();
    params = params.append('date', date.toDateString());
    this.http.get(this.finalApi, { params: params })
      .subscribe(response => {
        console.log(response);
        this.apiData = response as Energy[];
        console.log(this.apiData);
        this.chartOptionC = this.setChartDataC(this.apiData);
        this.chartOptionV = this.setChartDataV(this.apiData);
        this.chartOptionP = this.setChartDataP(this.apiData);
      }, err => {
        console.log(err);
      });
  }

  setChartDataV(value: Energy[]) {

    this.chartV = {
      tooltip: {},

      radius: '100%',
      xAxis: {
        data: this.apiData.map((currElement, index) => {
          return index; //equivalent to list[index]
        })
      },
      yAxis: {
        type: 'value',
      },
      series: [
        {
          data: this.apiData.map(x => x.voltage),
          type: 'line',
        },
      ],
    };
    return this.chartV;

  }
  setChartDataP(value: Energy[]) {

    this.chartP = {
      tooltip: {},

      radius: '100%',
      xAxis: {
        data: this.apiData.map((currElement, index) => {
          return index; //equivalent to list[index]
        }),
      },
      yAxis: {
        type: 'value',
      },
      series: [
        {
          data: this.apiData.map(x => x.power),
          type: 'line',
        },
      ],
    };
    return this.chartP;

  }
  setChartDataC(value: Energy[]) {

    this.chartC = {
      tooltip: {},

      radius: '100%',
      xAxis: {
        data: this.apiData.map((currElement, index) => {
          return index; //equivalent to list[index]
        })
      },
      yAxis: {
        type: 'value',
      },
      series: [
        {
          data: this.apiData.map(x => x.current),
          type: 'line',
        },
      ],
    };
    return this.chartC;

  }
}
