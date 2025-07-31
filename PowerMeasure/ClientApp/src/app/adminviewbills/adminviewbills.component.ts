import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable } from 'rxjs';

interface Bills {
  id?: number;
  billAmount: number;
  isPaid: boolean;
  dueDate: Date;
  forMonth: Date;
}

@Component({
  selector: 'app-adminviewbills',
  templateUrl: './adminviewbills.component.html',
  styleUrls: ['./adminviewbills.component.css']
})
export class AdminviewbillsComponent implements OnInit {
  bills: Bills[];
  page = 1;
  pageSize = 7;
  collectionSize = 3;
  countries$: Observable<Bills[]>;
  filter = new FormControl('', { nonNullable: true });
  slicedBills: any;
  @Input() id: number;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getBills();
  }

  getBills() {
    let apiUrl = 'https://localhost:5001/api/payment/getBills/';
    let finalApi = apiUrl + this.id;
    this.http.get(finalApi).subscribe(
      response => {
        console.log(response);
        this.bills = response as Bills[];
        this.slicedBills = this.bills;
        this.sliceData();
        this.collectionSize = this.bills.length;
      },
      err => {
        console.log(err);
      }
    );
  }

  sliceData() {
    this.slicedBills = this.bills.map((user) => ({ ...user }))
      .slice((this.page - 1) * this.pageSize, (this.page - 1) * this.pageSize + this.pageSize);
  }

}
