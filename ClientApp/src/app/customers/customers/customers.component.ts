import { Component, OnInit, PipeTransform } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';

import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { UsersService } from 'src/app/service/users.service';

interface Users {
  id?: number;
  firstName: string;
  lastName: string;
  emailAddress: string;
  isActive: boolean;
}

@Component({
  selector: 'app-customers',
  templateUrl: './customers.component.html',
  styleUrls: ['./customers.component.css'],
})
export class CustomersComponent implements OnInit {
  users: Users[];
  page = 1;
  pageSize = 4;
  collectionSize = 3;
  countries$: Observable<Users[]>;
  filter = new FormControl('', { nonNullable: true });
  slicedData: any;
  usrCount: any;
  adminCount: any;
  constructor(private service: UsersService, private router: Router) { }

  ngOnInit(): void {
    this.getUsersCount();
    this.getAdminCount();
    this.refreshUsers();


    this.countries$ = this.filter.valueChanges.pipe(
      startWith(''),
      map(text => this.search(text)),
    );
  }
  getUsersCount() {
    this.service.getUsersCount()
      .subscribe(response => {
        this.usrCount = response;
        console.log(this.usrCount);
      }, err => {
        console.log(err);
      }
      );
  }

  getAdminCount() {
    this.service.getAdminCount()
      .subscribe(response => {
        this.adminCount = response;
      }, err => {
        console.log(err);
      }
      );
  }

  gotoUser(index: number, userId: number) {

  }

  refreshUsers() {
    this.service.getUsers()
      .subscribe(response => {
        this.users = response as Users[];
        this.slicedData = this.users;
        this.sliceData();
        this.collectionSize = this.users.length;
      },
        err => {
          console.log(err);
        });
  }

  sliceData() {
    this.slicedData = this.users.map((user) => ({ ...user }))
      .slice((this.page - 1) * this.pageSize, (this.page - 1) * this.pageSize + this.pageSize);
  }

  search(text: string): Users[] {
    return this.users.filter(user => {
      const term = text.toLowerCase();
      return user.firstName.toLowerCase().includes(term)
        || user.lastName.toLowerCase().includes(term)
        || user.emailAddress.toLowerCase().includes(term);
    });
  }

  redirectToAddUserForm() {
    this.router.navigateByUrl("/add-user")
  }
}
