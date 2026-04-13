import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '@users/services/auth.service';

export const publisherGuard: CanActivateFn = () => {
  const authService = inject(AuthService);

  if (authService.isPublisher() || authService.isAdmin()) {
    return true;
  }

  console.warn('User does not have publisher or admin role');
  inject(Router).navigate(['/']);
  return false;
};
