import { Component, ComponentFactoryResolver, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../service/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
    loginForm: FormGroup;
    submitted = false;
    returnUrl: string;
    loginValid = true;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService : AuthService
  ) { }

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      email: ['', Validators.compose([Validators.required, Validators.email])],
      password: ['', Validators.required]
  });
  this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  get formControls() { return this.loginForm.controls; }


  onSubmit() {
    this.submitted = true;
    const credetials = {
      "EmailAddress": this.loginForm.controls['email'].value,
      "Password": this.loginForm.controls['password'].value
    }
      if (this.loginForm.invalid) {
          return;
      }

      this.authService.login(credetials)
      .subscribe(response => {
        const token = (<any>response).token;
        localStorage.setItem("jwt", token);
        let role = this.authService.getRoleFromToken();
        if(role == 'user') {
          this.router.navigate(["/dashboard"]);
        }
        else {
          this.router.navigate(["/customers"]);

        }
        
      }, err=> {
        console.log(err);
        this.loginValid = false;
      }
      )
  }

}
