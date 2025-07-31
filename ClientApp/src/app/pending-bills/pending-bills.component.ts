import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../service/auth.service';

interface Bills {
  id?: number;
  billAmount: number;
  isPaid: boolean;
  dueDate: Date;
  forMonth: Date;
}

@Component({
  selector: 'app-pending-bills',
  templateUrl: './pending-bills.component.html',
  styleUrls: ['./pending-bills.component.css']
})
export class PendingBillsComponent implements OnInit {
  bills: Bills[];

  constructor(private service: AuthService,
    private http: HttpClient) { }

  ngOnInit(): void {
    this.getPendingBills();
  }

  getPendingBills() {
    let id = this.service.getUserIdFromToken();
    let apiUrl = 'https://localhost:5001/api/payment/getUnpaidBills/';
    let finalApi = apiUrl + id;
    this.http.get(finalApi).subscribe(
      response => {
        this.bills = response as Bills[];
      },
      err => {
        console.log(err);
      }  
    );
  }

}
