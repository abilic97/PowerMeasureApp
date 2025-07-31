import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Observable } from 'rxjs';
import { PaymentComponent } from '../payment/payment.component';
import { AuthService } from '../service/auth.service';

interface Bills {
  id?: number;
  billAmount: number;
  isPaid: boolean;
  dueDate: Date;
  forMonth: Date;
}
@Component({
  selector: 'app-bills',
  templateUrl: './bills.component.html',
  styleUrls: ['./bills.component.css']
})
export class BillsComponent implements OnInit {
  bills: Bills[];
  page = 1;
  pageSize = 7;
  collectionSize = 3;
  countries$: Observable<Bills[]>;
  filter = new FormControl('', { nonNullable: true });
  slicedBills: any;

  constructor(private service: AuthService,
    private http: HttpClient,
    private modalService: NgbModal) { }

  ngOnInit(): void {
    this.getBills();
  }

  open(index: number, billId: number) {
    console.log(index, billId);
    const modalRef = this.modalService.open(PaymentComponent);
    
    modalRef.componentInstance.billId = billId;
    modalRef.result.then((data) => {
      this.getBills();
    }, (reason) => {
      this.getBills();
    });
  }

  getBills() {
    let id = this.service.getUserIdFromToken();
    let apiUrl = 'https://localhost:5001/api/payment/getBills/';
    let finalApi = apiUrl + id;
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
