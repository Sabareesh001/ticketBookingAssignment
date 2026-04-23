import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { OperatorAuthService } from '../../services/operator-auth.service';
import { LoginRequest } from '../../models/auth.model';
import { OperatorLoginRequest } from '../../models/operator-auth.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  operatorLoginForm!: FormGroup;
  loading = false;
  submitted = false;
  error = '';
  returnUrl = '';
  isOperatorLogin = false;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private operatorAuthService: OperatorAuthService
  ) {}

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';

    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    this.operatorLoginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  get f() {
    return this.loginForm.controls;
  }

  get operatorF() {
    return this.operatorLoginForm.controls;
  }

  toggleOperatorLogin(): void {
    this.isOperatorLogin = !this.isOperatorLogin;
    this.error = '';
    this.submitted = false;
    if (this.isOperatorLogin) {
      this.operatorLoginForm.reset();
    } else {
      this.loginForm.reset();
    }
  }

  onSubmit(): void {
    this.submitted = true;
    this.error = '';

    if (this.isOperatorLogin) {
      this.submitOperatorLogin();
    } else {
      this.submitUserLogin();
    }
  }

  private submitUserLogin(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;
    const request: LoginRequest = {
      email: this.f['email'].value,
      password: this.f['password'].value
    };

    this.authService.login(request).subscribe({
      next: (response) => {
        if (response.success) {
          this.router.navigateByUrl(this.returnUrl);
        }
      },
      error: (error) => {
        this.error = error.message || 'Login failed';
        this.loading = false;
      }
    });
  }

  private submitOperatorLogin(): void {
    if (this.operatorLoginForm.invalid) {
      return;
    }

    this.loading = true;
    const request: OperatorLoginRequest = {
      email: this.operatorF['email'].value,
      password: this.operatorF['password'].value
    };

    this.operatorAuthService.login(request).subscribe({
      next: (response) => {
        this.router.navigate(['/operator-dashboard']);
      },
      error: (error) => {
        this.error = error.message || 'Login failed';
        this.loading = false;
      }
    });
  }
}
