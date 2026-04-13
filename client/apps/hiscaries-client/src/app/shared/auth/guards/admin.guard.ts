import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '@users/services/auth.service';

export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);

  if (authService.isAdmin()) {
    return true;
  }

  inject(Router).navigate(['/']);
  return false;
};
