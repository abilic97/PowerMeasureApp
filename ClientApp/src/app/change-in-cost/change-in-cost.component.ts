import { Component, OnInit } from '@angular/core';
import { EChartsOption } from 'echarts';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-change-in-cost',
  templateUrl: './change-in-cost.component.html',
  styleUrls: ['./change-in-cost.component.css']
})
export class ChangeInCostComponent implements OnInit {
  theme: any;
  date = new Date();
 chartOption: EChartsOption = {
   title: {
     mainType: 'title',
     text: 'Total cost per month [HRK]',
     show: true,
   },
   tooltip: {
     formatter: "{a} <br/>{b}: {c}%"
 },

   radius: '100%',
   xAxis: {
     type: 'category',
     data: [this.date.getMonth()-1, this.date.getMonth()],
   },
   yAxis: {
     type: 'value',
     
   },
   series: [
     {
       data: [
        {
          value: 10,
          itemStyle: {color: '#cc0000'},
      },
      {
        value: 7,
        itemStyle: {color: 'green'},
    },
      ],
       type: 'bar',
       color: 'red'
     },
   ],
 };
  constructor() { }

  ngOnInit(): void {
    this.theme = environment.graphTheme;
  }

}
