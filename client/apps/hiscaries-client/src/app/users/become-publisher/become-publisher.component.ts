import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '@users/services/auth.service';
import { Router } from '@angular/router';
import { NavigationConst } from '@shared/constants/navigation.const';
import { UserService } from '@users/services/user.service';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

@Component({
  selector: 'app-become-publisher',
  standalone: true,
  imports: [CommonModule, ProgressSpinnerModule],
  templateUrl: './become-publisher.component.html',
  styleUrl: './become-publisher.component.scss',
})
export class BecomePublisherComponent {
  agreed = false;

  constructor(
    public authService: AuthService,
    private userService: UserService,
    private router: Router,
  ) {
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
