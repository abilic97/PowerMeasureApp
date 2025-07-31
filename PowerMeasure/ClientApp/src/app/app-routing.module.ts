import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminviewComponent } from './adminview/adminview.component';
import { BillsComponent } from './bills/bills.component';
import { CustomersComponent } from './customers/customers/customers.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AuthGuard } from './guard/auth.guard';
import { RoleGuard } from './guard/role.guard';
import { LoginComponent } from './login/login.component';
import { PaymentComponent } from './payment/payment.component';
import { StatisticsComponent } from './statistics/statistics.component';
import { TestComponent } from './test/test.component';
import { UpsertUserComponent } from './upsert-user/upsert-user.component';

const routes: Routes = [
  {path:'', redirectTo:'login', pathMatch:'full'},
  {path:'login', component: LoginComponent},
  {path: 'pay', component: PaymentComponent },
  {path:'statistics', component: StatisticsComponent/*, canActivate:[AuthGuard]*/},
  {path:'test', component: TestComponent/*, canActivate:[AuthGuard]*/},
  {path:'dashboard', component: DashboardComponent, canActivate:[AuthGuard]},
  {path:'bills', component: BillsComponent, canActivate:[AuthGuard]},
  { path : 'person/:id', component : AdminviewComponent},
  
  {path:'customers', component: CustomersComponent , canActivate:[RoleGuard]},
  {path:'add-user', component: UpsertUserComponent /*, canActivate:[RoleGuard]*/}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
