import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Monthlyconsumption } from '../monthlyconsumption';
import { AuthService } from '../service/auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  mothlyApiData: Monthlyconsumption[];

  constructor(private service: AuthService, private http: HttpClient) { }

  ngOnInit(): void {
    this.fetchMonthlyStats();
  }

  fetchMonthlyStats() {
    let id = this.service.getUserIdFromToken();
    let apiUrl = 'https://localhost:5001/api/consumption/get-monthly-total/';
    let finalApi = apiUrl + id;

    this.http.get(finalApi).subscribe(
      response => {
        this.mothlyApiData = response as Monthlyconsumption[];
      },
      err => {
        console.log(err);
      }
    );
  }

}
