import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit, Input } from '@angular/core';
import { FormControl } from '@angular/forms';
import { EChartsOption } from 'echarts';
import { ThemeOption } from 'ngx-echarts';
import { environment } from 'src/environments/environment';
import { AuthService } from '../service/auth.service';
interface Energy {
  current: number,
  voltage: number,
  power: number,
  reportDate: Date
}

@Component({
  selector: 'app-dailypowerstat',
  templateUrl: './dailypowerstat.component.html',
  styleUrls: ['./dailypowerstat.component.css']
})
export class DailypowerstatComponent implements OnInit {
  @Input() date: FormControl;
  @Input() userId: any;
  theme: any;
  chartOptionP: EChartsOption;
  chartOptionV: EChartsOption;
  chartOptionC: EChartsOption;
  chart: EChartsOption;
  apiData: Energy;
  finalApi: string;
  constructor(private service: AuthService, private http: HttpClient) { }

  ngOnInit(): void {
    const currentDate = new Date();
    currentDate.setHours(0, 0, 0);
    this.setTheme();

    this.getDailyPowerStat(currentDate);
    this.date.valueChanges.subscribe(val => {
      console.log(val, "here");
      if(typeof val != "undefined") {
        let selectedDate = new Date(val!.year, val!.month-1, val!.day, 0, 0 ,0);
        console.log(selectedDate, "test");
        this.getDailyPowerStat(selectedDate);
      }
    });
  }

  setTheme() {
    this.theme = environment.graphTheme;
  }

  getDailyPowerStat(date: Date) {
    let apiUrl = 'https://localhost:5001/api/consumption/getLastPowerValue/';
    if(this.userId == null)
    {
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
        this.apiData = response as Energy;
        console.log(this.apiData);
        this.chartOptionC = this.setChartData(this.apiData.current, this.apiData.reportDate, "Latest Current Value [A]");
        this.chartOptionV = this.setChartData(this.apiData.voltage, this.apiData.reportDate, "Latest Voltage Value [V]");
        this.chartOptionP = this.setChartData(this.apiData.power / 1000, this.apiData.reportDate, "Latest Power Value [kWh]");
      }, err => {
        console.log(err);
      });
  }

  setChartData(value: any, d: any, title: string) {
    console.log(value, d);
    this.chart = {
      radius: '100%',
      xAxis: {
        type: 'time',
        data: [d],
      },
      yAxis: {
        type: 'value',
      },
      series: [
        {
          data: [Math.round(value * 100) / 100],
          type: 'gauge',
          max: 220,
        },
      ],
    };
    return this.chart;
  }
}
