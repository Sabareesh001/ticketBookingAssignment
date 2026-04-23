import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { OperatorAuthService } from '../../services/operator-auth.service';
import { OperatorSignupRequest } from '../../models/operator-auth.model';

@Component({
  selector: 'app-operator-signup',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './operator-signup.component.html',
  styleUrls: ['./operator-signup.component.css']
})
export class OperatorSignupComponent implements OnInit {
  signupForm!: FormGroup;
  loading = false;
  submitted = false;
  error = '';

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private operatorAuthService: OperatorAuthService
  ) {}

  ngOnInit(): void {
    this.signupForm = this.formBuilder.group({
      operatorName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
      licenseNumber: ['', [Validators.required, Validators.minLength(5)]],
      address: ['', [Validators.required, Validators.minLength(5)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });
  }

  get f() {
    return this.signupForm.controls;
  }

  passwordMatchValidator(group: FormGroup): { [key: string]: any } | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;

    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  onSubmit(): void {
    this.submitted = true;
    this.error = '';

    if (this.signupForm.invalid) {
      return;
    }

    this.loading = true;
    const formValue = this.signupForm.value;

    const request: OperatorSignupRequest = {
      operatorName: formValue.operatorName,
      email: formValue.email,
      phoneNumber: formValue.phoneNumber,
      licenseNumber: formValue.licenseNumber,
      address: formValue.address,
      password: formValue.password
    };

    this.operatorAuthService.signup(request).subscribe({
      next: (response) => {
        this.router.navigate(['/operator-dashboard']);
      },
      error: (error) => {
        this.error = error.message || 'Signup failed';
        this.loading = false;
      }
    });
  }
}
