import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { Monthlyconsumption } from '../monthlyconsumption';
import { AuthService } from '../service/auth.service';


@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.css']
})
export class StatisticsComponent implements OnInit {
  model: NgbDateStruct;
  showDatePicker: boolean;
  isMonthView: boolean;
  isDayView: boolean;
  isIndividalActive: boolean;
  mothlyApiData: Monthlyconsumption[];
  formGroup = new FormGroup({
    date: new FormControl({ day: 20, month: 4, year: 1969 }, [Validators.required])
  });

  constructor(private service: AuthService, private http: HttpClient) { }

  ngOnInit(): void {
    this.formGroup.get("date")!.valueChanges.subscribe(val => {
    });

    this.showDatePicker = false;

    this.isMonthView = true;
    this.isDayView = false;
    this.fetchMonthlyStats();
  }

  get formControls() { return this.formGroup.controls; }

  selectDate() {
    if (this.showDatePicker == true) {
      this.showDatePicker = false;
    }
    else {
      this.showDatePicker = true;
    }
    this.isMonthView = false;
    this.isDayView = true;
  }

  selectMonthView() {
    this.isDayView = false;
    this.showDatePicker = false;
    this.isMonthView = true;
  }
  selectIndividual() {
    if (this.showDatePicker == true) {
      this.showDatePicker = false;
    }
    else {
      this.showDatePicker = true;
    }
    this.isMonthView = false;
    this.isDayView = false;
    this.isIndividalActive = true;
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
