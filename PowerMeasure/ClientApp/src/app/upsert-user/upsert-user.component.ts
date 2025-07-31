import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-upsert-user',
  templateUrl: './upsert-user.component.html',
  styleUrls: ['./upsert-user.component.css']
})
export class UpsertUserComponent implements OnInit {
  addUserForm: FormGroup;
  isUserActive: boolean;
  activeDisplayMsg = "User Active";
  submitted: boolean;

  userRole = "User Role";

  isEmActive: boolean;
  emActiveMessage = "EM Active";
  cvfmodel: NgbDateStruct;
  cvtmodel: NgbDateStruct;
  emfmodel: NgbDateStruct;
  emtmodel: NgbDateStruct;

  constructor(private formBuilder: FormBuilder, private http: HttpClient) { }

  ngOnInit(): void {
    this.addUserForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', Validators.compose([Validators.required, Validators.email])],
      password: ['', Validators.required],
      userActive: [null, Validators.required],
      role: ['', Validators.required],
      contractValidFrom: ['', Validators.required],
      contractValidTo: ['', Validators.required],
      emValidFrom: ['', Validators.required],
      emValidTo: ['', Validators.required],
      emCode: ['', Validators.required],
      emActive: [null, Validators.required],
    });
  }

  get formControls() { return this.addUserForm.controls; }

  selectUserActive(selectedOption: string) {
    if (selectedOption === 'False') {
      this.isUserActive = false;
      this.activeDisplayMsg = 'False'
    }
    else {
      this.isUserActive = true;
      this.activeDisplayMsg = 'True';
    }
    this.formControls['userActive'].setValue(this.isUserActive);
  }

  selectUserRole(selectedOption: string) {
    this.formControls['role'].setValue(selectedOption);
    if (selectedOption == 'Admin') {
      this.userRole = 'Admin';
    }
    else {
      this.userRole = 'User'
    }
  }

  selectEmActive(selectedOption: string) {
    if (selectedOption == 'False') {
      this.isEmActive = false;
      this.emActiveMessage = 'False'
    }
    else {
      this.isEmActive = true;
      this.emActiveMessage = 'True';
    }
    this.formControls['emActive'].setValue(this.isEmActive);
  }

  submitUser() {
    this.submitted = true;
    const apiUrl = "https://localhost:5001/api/user/createUser";
    console.log(this.addUserForm);
    let cvf = new Date(this.formControls['contractValidFrom'].value.year,
      this.formControls['contractValidFrom'].value.month - 1,
      this.formControls['contractValidFrom'].value.day,
      0, 0, 0);
    let cvt = new Date(
      this.formControls['contractValidTo'].value.year,
      this.formControls['contractValidTo'].value.month - 1,
      this.formControls['contractValidTo'].value.day,
      0, 0, 0);
    let emvf = new Date(this.formControls['emValidFrom'].value.year,
      this.formControls['emValidFrom'].value.month - 1,
      this.formControls['emValidFrom'].value.day,
      0, 0, 0);
    let emvt = new Date(this.formControls['emValidTo'].value.year,
      this.formControls['emValidTo'].value.month - 1,
      this.formControls['emValidTo'].value.day,
      0, 0, 0);

    const postData = {
      "FirstName": this.formControls['firstName'].value,
      "LastName": this.formControls['lastName'].value,
      "Email": this.formControls['email'].value,
      "Password": this.formControls['password'].value,
      "IsUserActive": this.formControls['userActive'].value,
      "RoleName": this.formControls['role'].value,
      "ContractValidFrom": cvf.toDateString(),
      "ContractValidTo": cvt.toDateString(),
      "EmContractValidFrom": emvf.toDateString(),
      "EmContractValidTo": emvt.toDateString(),
      "EmCode": this.formControls['emCode'].value,
      "EmActive": this.formControls['emActive'].value
    }
    this.http.post(apiUrl, postData).subscribe(response => {
      alert("User was added successfully");

    }, err => {
      alert("Something went wrong! Please try again later");

    }
    );
  }
}
