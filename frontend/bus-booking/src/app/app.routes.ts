import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { SignupComponent } from './pages/signup/signup.component';
import { ForgotPasswordComponent } from './pages/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './pages/reset-password/reset-password.component';
import { UnauthorizedComponent } from './pages/unauthorized/unauthorized.component';
import { AuthGuardService } from './guards/auth.guard';
import { UserRole } from './models/auth.model';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
  { path: 'unauthorized', component: UnauthorizedComponent },
  
  // Protected routes - add your dashboard and other protected routes here
  // Example:
  // { 
  //   path: 'dashboard', 
  //   component: DashboardComponent,
  //   canActivate: [AuthGuardService],
  //   data: { roles: [UserRole.USER, UserRole.ADMIN, UserRole.BUS_OPERATOR] }
  // },
  // {
  //   path: 'admin',
  //   component: AdminComponent,
  //   canActivate: [AuthGuardService],
  //   data: { roles: [UserRole.ADMIN] }
  // },
  // {
  //   path: 'operator',
  //   component: OperatorComponent,
  //   canActivate: [AuthGuardService],
  //   data: { roles: [UserRole.BUS_OPERATOR] }
  // }
];
