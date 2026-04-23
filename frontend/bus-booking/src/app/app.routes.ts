import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { SignupComponent } from './pages/signup/signup.component';
import { ForgotPasswordComponent } from './pages/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './pages/reset-password/reset-password.component';
import { UnauthorizedComponent } from './pages/unauthorized/unauthorized.component';
import { Dashboard } from './pages/dashboard/dashboard';
import { OperatorSignupComponent } from './pages/operator-signup/operator-signup.component';
import { OperatorDashboardComponent } from './pages/operator-dashboard/operator-dashboard.component';
import { AuthGuardService } from './guards/auth.guard';
import { AuthRedirectGuard } from './guards/auth-redirect.guard';
import { OperatorAuthGuard } from './guards/operator-auth.guard';
import { UserRole } from './models/auth.model';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent, canActivate: [AuthRedirectGuard] },
  { path: 'signup', component: SignupComponent, canActivate: [AuthRedirectGuard] },
  { path: 'forgot-password', component: ForgotPasswordComponent, canActivate: [AuthRedirectGuard] },
  { path: 'reset-password', component: ResetPasswordComponent, canActivate: [AuthRedirectGuard] },
  { path: 'unauthorized', component: UnauthorizedComponent },
  
  // Operator routes
  { path: 'operator-signup', component: OperatorSignupComponent, canActivate: [AuthRedirectGuard] },
  { 
    path: 'operator-dashboard', 
    component: OperatorDashboardComponent,
    canActivate: [OperatorAuthGuard]
  },
  
  // Protected routes
  { 
    path: 'dashboard', 
    component: Dashboard,
    canActivate: [AuthGuardService],
    data: { roles: [UserRole.USER, UserRole.ADMIN, UserRole.BUS_OPERATOR] }
  }
];
