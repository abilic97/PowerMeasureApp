import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  usersApiUrl = 'https://localhost:5001/api/user/users';
  usercountApi = 'https://localhost:5001/api/user/get-users-count'
  admincountApi = 'https://localhost:5001/api/admin/get-admin-count'


  constructor(private http: HttpClient) { }

  getUsers() {
    return this.http.get(this.usersApiUrl);
  }

  getUsersCount() {
   return this.http.get(this.usercountApi);
  }

  getAdminCount() {
    return this.http.get(this.usercountApi);
   }
}
