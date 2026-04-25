import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { OperatorAuthService } from '../../services/operator-auth.service';
import { OperatorLoginRequest } from '../../models/operator-auth.model';

@Component({
  selector: 'app-operator-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './operator-login.component.html',
  styleUrls: ['./operator-login.component.css']
})
export class OperatorLoginComponent implements OnInit {
  loginForm!: FormGroup;
  loading = false;
  submitted = false;
  error = '';

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private operatorAuthService: OperatorAuthService
  ) {}

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  get f() {
    return this.loginForm.controls;
  }

  onSubmit(): void {
    this.submitted = true;
    this.error = '';

    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;
    const formValue = this.loginForm.value;

    const request: OperatorLoginRequest = {
      email: formValue.email,
      password: formValue.password
    };

    this.operatorAuthService.login(request).subscribe({
      next: (response) => {
        console.log('Login successful:', response);
        this.router.navigate(['/operator-dashboard']);
      },
      error: (error) => {
        this.error = error.message || 'Login failed';
        this.loading = false;
      }
    });
  }
}
