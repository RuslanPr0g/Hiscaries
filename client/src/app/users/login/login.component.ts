import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { take } from 'rxjs';
import { NavigationConst } from '../../shared/constants/navigation.const';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { PasswordModule } from 'primeng/password';
import { CheckboxModule } from 'primeng/checkbox';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

@Component({
    standalone: true,
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        CardModule,
        InputTextModule,
        PasswordModule,
        CheckboxModule,
        ButtonModule,
        ProgressSpinnerModule,
    ],
})
export class LoginComponent implements OnInit {
    formLogin: FormGroup;
    formRegister: FormGroup;
    isLoginState = true;
    errorMessage: string | null = null;
    isLoading: boolean = true;

    constructor(
        private fb: FormBuilder,
        private authService: AuthService,
        private router: Router
    ) {
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

        setTimeout(() => (this.isLoading = false), 1000);
    }

    onSubmit(): void {
        const form = this.isLoginState ? this.formLogin : this.formRegister;

        if (form.invalid) {
            this.errorMessage = 'Please fill all required fields!';
            return;
        }

        const action = this.isLoginState ? this.authService.login(form.value) : this.authService.register(form.value);

        action.pipe(take(1)).subscribe({
            next: () => this.router.navigateByUrl(NavigationConst.Home),
            error: (err) => {
                this.errorMessage = err.error.Message || (this.isLoginState ? 'Login failed' : 'Registration failed');
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
