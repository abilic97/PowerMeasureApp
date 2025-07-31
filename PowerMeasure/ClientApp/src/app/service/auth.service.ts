import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  apiUrl = 'https://localhost:5001/api/user/login/';

  constructor(private http: HttpClient, private router: Router) { }

  login(credetials: any) {
    return this.http.post(this.apiUrl, credetials);
  }

  isUserLoggedIn() {
    return localStorage.getItem("jwt") != null;
  }

  getToken() {
    return localStorage.getItem("jwt")||'';
  }

  hasAdminAcces() {
    let token = localStorage.getItem("jwt")||'';
    let extractedToken = token.split('.')[1];
    let atobData = atob(extractedToken);
    let tokenData = JSON.parse(atobData);
    if(tokenData['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] == 'admin') {
      return true;
    }
    return false;
  }

  logOut() {
    localStorage.clear();
    this.router.navigateByUrl('/login');
  }

  getRoleFromToken() {
    let token = localStorage.getItem("jwt")||'';
    let extractedToken = token.split('.')[1];
    let atobData = atob(extractedToken);
    let tokenData = JSON.parse(atobData);
    console.log(tokenData);
    return tokenData['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
  }

  getUserFromToken() {
    let token = localStorage.getItem("jwt")||'';
    let extractedToken = token.split('.')[1];
    let atobData = atob(extractedToken);
    let tokenData = JSON.parse(atobData);
    let firstName = tokenData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"];
    let lastName = tokenData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"];
    return firstName + ' ' + lastName;
  }

  getUserIdFromToken() {
    let token = localStorage.getItem("jwt")||'';
    let extractedToken = token.split('.')[1];
    let atobData = atob(extractedToken);
    let tokenData = JSON.parse(atobData);
    let id = tokenData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid"];
    return id;
  }
}
