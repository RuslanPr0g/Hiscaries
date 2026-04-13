import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '@users/services/auth.service';
import { take } from 'rxjs';
import { NavigationConst } from '@shared/constants/navigation.const';


import { FormsModule } from '@angular/forms';

import { Button } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { Card } from 'primeng/card';
import { Password } from 'primeng/password';
import { LoadingSpinnerComponent } from '@shared/components/loading-spinner/loading-spinner.component';

@Component({
  standalone: true,
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    Card,
    InputText,
    Password,
    Button,
    LoadingSpinnerComponent
],
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  formLogin: FormGroup;
  formRegister: FormGroup;
  isLoginState = true;
  errorMessage: string | null = null;
  isLoading = true;
  isLoadedFully = false;

  constructor() {
    this.formLogin = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
    });

    this.formRegister = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      email: ['', [Validators.required, Validators.email]],
    });
  }

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.router.navigateByUrl(NavigationConst.Home);
    }

    setTimeout(() => {
      this.isLoading = false;
      this.isLoadedFully = true;
    }, 1000);
  }

  onSubmit(): void {
    if (this.isLoading) {
      console.warn('loading');

      return;
    }

    this.isLoading = true;

    const form = this.isLoginState ? this.formLogin : this.formRegister;

    if (form.invalid) {
      this.errorMessage = 'Please fill all required fields!';
      return;
    }

    const action = this.isLoginState
      ? this.authService.login(form.value)
      : this.authService.register(form.value);

    action.pipe(take(1)).subscribe({
      next: () => this.router.navigateByUrl(NavigationConst.Home),
      error: (err) => {
        this.errorMessage =
          err.error.Message || (this.isLoginState ? 'Login failed' : 'Registration failed');
        this.isLoading = false;
      },
    });
  }

  toggleState(): void {
    this.isLoginState = !this.isLoginState;
    this.errorMessage = null;
    this.formLogin.reset();
    this.formRegister.reset();
  }
}
