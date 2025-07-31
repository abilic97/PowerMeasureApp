import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit, Input } from '@angular/core';
import { FormControl } from '@angular/forms';
import { EChartsOption } from 'echarts';
import { ThemeOption } from 'ngx-echarts';
import { forkJoin, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { AuthService } from '../service/auth.service';

interface Energy {
  c: number,
  v: number,
  p: number,
}

@Component({
  selector: 'app-daily-total',
  templateUrl: './daily-total.component.html',
  styleUrls: ['./daily-total.component.css']
})
export class DailyTotalComponent implements OnInit {
  @Input() date: FormControl;
  @Input() userId: any;
  theme: any;
  chartOption: EChartsOption;
  apidata: Energy;
  apiDatay: Energy;
  finalApi: string;

  constructor(private service: AuthService, private http: HttpClient) { }

  ngOnInit(): void {
    const currentDate = new Date();
    currentDate.setHours(0, 0, 0);
    const yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);
    yesterday.setHours(0, 0, 0);

    this.setTheme();

    this.getDailyPowerStat(currentDate, yesterday);
    this.date.valueChanges.subscribe(val => {
      console.log(val, "here");
      if (typeof val != "undefined") {
        let selectedDate = new Date(val!.year, val!.month - 1, val!.day, 0, 0, 0);
        console.log(selectedDate, "test");
        let daybefore =new Date();
        daybefore.setDate(selectedDate.getDate() - 1);
        daybefore.setHours(0, 0, 0);
        // this.getDailyPowerStat(selectedDate);
        this.getDailyPowerStat(selectedDate, daybefore);
      }
    });
  }

  setTheme() {
    this.theme = environment.graphTheme;
  }

  getcurrentdaily(date: Date): Observable<any> {
    let apiUrl = 'https://localhost:5001/api/consumption/getTotalDailyEnergy/';
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
    return this.http.get<any>(this.finalApi, { params: params });
  }

  getyesterdaydaily(date: Date): Observable<any> {
    let apiUrl = 'https://localhost:5001/api/consumption/getTotalDailyEnergy/';
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
    return this.http.get<any>(this.finalApi, { params: params });
  }

  getDailyPowerStat(date: Date, yd: Date) {
    forkJoin([this.getcurrentdaily(date),
    this.getyesterdaydaily(yd)
    ]).subscribe(([data, ydData]: any) => {
      // do stuff with data recieved
      console.log(data, ydData, "testtest");
      this.apidata = data as Energy;
      this.apiDatay = ydData as Energy;
      this.initializeGraph(date, yd);
    });
  }

  initializeGraph(date: Date, yd: Date) {
    let dateC = date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate();
    let dateY = yd.getFullYear() + '/' + (yd.getMonth() + 1) + '/' + yd.getDate();

    this.chartOption = {
      tooltip: {},

      radius: '100%',
      xAxis: {
        data: [dateY, dateC]

      },
      yAxis: {
        type: 'value',
      },
      series: [
        {
          data: [this.apiDatay.c, this.apidata.c],
          type: 'bar',
          xAxisIndex: 0,
          name: 'Current'
        },
        {
          data: [this.apiDatay.v, this.apidata.v],
          type: 'bar',
          xAxisIndex: 0,
          name: 'Voltage'
        },
        {
          data: [this.apiDatay.p, this.apidata.p],
          type: 'bar',
          xAxisIndex: 0,
          name: 'Power'
        },
      ],
    };

  }

}
