import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { LoadingSpinnerComponent } from '@shared/components/loading-spinner/loading-spinner.component';
import { NavigationConst } from '@shared/constants/navigation.const';
import { AuthService } from '@users/services/auth.service';
import { UserService } from '@users/services/user.service';

@Component({
  selector: 'app-become-publisher',
  standalone: true,
  imports: [LoadingSpinnerComponent],
  templateUrl: './become-publisher.component.html',
  styleUrl: './become-publisher.component.scss',
})
export class BecomePublisherComponent {
  authService = inject(AuthService);
  private userService = inject(UserService);
  private router = inject(Router);

  agreed = false;

  constructor() {
    if (this.authService.isPublisher()) {
      console.warn('User is already a publisher!');
      this.router.navigate([NavigationConst.Home]);
      return;
    }
  }

  confirmPublisher(): void {
    this.agreed = true;

    this.userService.becomePublisher().subscribe({
      next: () => {
        setTimeout(() => {
          this.authService.logOut();
          this.router.navigate([NavigationConst.Login]);
        }, 3000);
      },
      error: () => {
        this.agreed = false;
      },
    });
  }
}
