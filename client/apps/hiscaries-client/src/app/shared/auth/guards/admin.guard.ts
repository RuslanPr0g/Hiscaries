import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '@users/services/auth.service';

export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);

  if (authService.isAdmin()) {
    return true;
  }

  console.warn('User does not have admin role');
  inject(Router).navigate(['/']);
  return false;
};
