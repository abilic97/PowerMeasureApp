import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Params, Router, RouterEvent } from '@angular/router';
import { filter, Subscription } from 'rxjs';
import { AuthService } from 'src/app/service/auth.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {
  // @Input() displaymenu: boolean;
  currentRole: any;
  isAdmin = false;
  userName: any;
  active: number;
  private route$: Subscription;
  constructor(private service: AuthService, private router: Router,
    private route: ActivatedRoute) { 
      this.router.events.pipe(
        filter((e: any): e is RouterEvent => e instanceof RouterEvent)
      ).subscribe((evt: RouterEvent) => {
        if (evt instanceof NavigationEnd) {
          console.log(evt.url)
          if(evt.url == '/bills') {
            this.active = 2;
          }
          else if(evt.url == '/statistics') {
            this.active = 3;
          }
          else {
            this.active =1;
          }
        }
      });
    }

  ngOnInit(): void {
    this.getUser();
    ("HERE",this.userName)
    this.adminDisplay();
  }
  ngOnDestroy() {
    if (this.route$) this.route$.unsubscribe();
}

  getUser () {
    this.userName = this.service.getUserFromToken();
  }

  adminDisplay() {
    this.currentRole = this.service.getRoleFromToken();
    console.log(this.currentRole);
    if(this.currentRole == 'admin') {
      this.isAdmin = true;
    }
    else {
      this.isAdmin = false;
    }
  }
  onLogout() {
    this.service.logOut();
  }
  redirectToCustomersPage() {
    this.router.navigateByUrl("/customers")
  }
  redirectToDashboardPage() {
    this.router.navigateByUrl("/dashboard")
  }
}
