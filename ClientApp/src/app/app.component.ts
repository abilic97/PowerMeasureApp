import { Component, DoCheck } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements DoCheck {
  title = 'power_measure';
  displaymenu: boolean

  constructor(private route: Router) {}
  ngDoCheck(): void {
    if(this.route.url =='/login')
    {
      this.displaymenu = false;
    }
    else {
      this.displaymenu = true;
    }

  }
}

