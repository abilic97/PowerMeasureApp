import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import { TokenInterceptorService } from './service/token-interceptor.service';
import { MenuComponent } from './menu/menu/menu.component';
import {NgbModule} from "@ng-bootstrap/ng-bootstrap";
import { CustomersComponent } from './customers/customers/customers.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UpsertUserComponent } from './upsert-user/upsert-user.component';
import { NgxEchartsModule } from 'ngx-echarts';
import { DailypowerstatComponent } from './dailypowerstat/dailypowerstat.component';
import { FooterComponent } from './footer/footer.component';
import { TotalcostComponent } from './totalcost/totalcost.component';
import { TestComponent } from './test/test.component';
import { StatisticsComponent } from './statistics/statistics.component';
import { MothlypowerconsuptionComponent } from './mothlypowerconsuption/mothlypowerconsuption.component';
import { NoofPowerCutsComponent } from './noof-power-cuts/noof-power-cuts.component';
import { ChangeInCostComponent } from './change-in-cost/change-in-cost.component';
import { BillsComponent } from './bills/bills.component';
import { DailyCostComponent } from './daily-cost/daily-cost.component';
import { DynamicoptionsComponent } from './dynamicoptions/dynamicoptions.component';
import { DailyTotalComponent } from './daily-total/daily-total.component';
import { CostCalculatorLegendComponent } from './cost-calculator-legend/cost-calculator-legend.component';
import { NewsComponent } from './news/news.component';
import { PaymentComponent } from './payment/payment.component';
import { PendingBillsComponent } from './pending-bills/pending-bills.component';
import { AdminviewComponent } from './adminview/adminview.component';
import { AdminviewbillsComponent } from './adminviewbills/adminviewbills.component';
import { DailyIndividualLastValuesComponent } from './daily-individual-last-values/daily-individual-last-values.component';
import { DailyAllIndividualValuesComponent } from './daily-all-individual-values/daily-all-individual-values.component';


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DashboardComponent,
    MenuComponent,
    CustomersComponent,
    UpsertUserComponent,
    DailypowerstatComponent,
    FooterComponent,
    TotalcostComponent,
    TestComponent,
    StatisticsComponent,
    MothlypowerconsuptionComponent,
    NoofPowerCutsComponent,
    ChangeInCostComponent,
    BillsComponent,
    DailyCostComponent,
    DynamicoptionsComponent,
    DailyTotalComponent,
    CostCalculatorLegendComponent,
    NewsComponent,
    PaymentComponent,
    PendingBillsComponent,
    AdminviewComponent,
    AdminviewbillsComponent,
    DailyIndividualLastValuesComponent,
    DailyAllIndividualValuesComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
    [NgbModule],
    CommonModule,
    FormsModule,
    NgxEchartsModule.forRoot({
      /**
       * This will import all modules from echarts.
       * If you only need custom modules,
       * please refer to [Custom Build] section.
       */
      echarts: () => import('echarts'), // or import('./path-to-my-custom-echarts')
    }),
    ],
  providers: [{provide:HTTP_INTERCEPTORS, useClass:TokenInterceptorService, multi:true}],
  bootstrap: [AppComponent]
})
export class AppModule { }
