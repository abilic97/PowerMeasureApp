import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { Subscription } from 'rxjs';
import { Monthlyconsumption } from '../monthlyconsumption';
import { AuthService } from '../service/auth.service';

@Component({
  selector: 'app-adminview',
  templateUrl: './adminview.component.html',
  styleUrls: ['./adminview.component.css']
})
export class AdminviewComponent implements OnInit, OnDestroy  {
  model: NgbDateStruct;
  showDatePicker: boolean;
  isMonthView: boolean;
  isDayView: boolean;
  isBills: boolean;
  id: number;
  mothlyApiData: Monthlyconsumption[];
  private route$: Subscription;
  formGroup = new FormGroup({
    date: new FormControl({ day: 20, month: 4, year: 1969 }, [Validators.required])
  });
  constructor(private service: AuthService, private http: HttpClient,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route$ =  this.route.params.subscribe(
      (params: Params) => {
         this.id = params['id']; 
         console.log(this.id);
      }
   );

    this.showDatePicker = false;

    this.isMonthView = true;
    this.isDayView = false;
    this.isBills = false;
    this.fetchMonthlyStats();
  }
  ngOnDestroy() {
    if (this.route$) this.route$.unsubscribe();
}

  selectDate() {
    if (this.showDatePicker == true) {
      this.showDatePicker = false;
    }
    else {
      this.showDatePicker = true;
    }
    this.isMonthView = false;
    this.isBills=false;
    this.isDayView = true;

  }
  get formControls() { return this.formGroup.controls; }

  selectMonthView() {
    this.isDayView = false;
    this.showDatePicker = false;
    this.isMonthView = true;
    this.isBills=false;
  }
  selectBills() {
    this.isDayView = false;
    this.showDatePicker = false;
    this.isMonthView = false;
    this.isBills=true;
  }

  fetchMonthlyStats() {
    let apiUrl = 'https://localhost:5001/api/consumption/get-monthly-total/';
    let finalApi = apiUrl + this.id;

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
