import { inject } from '@angular/core';
import {
  CanActivateFn,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
} from '@angular/router';
import { AuthService } from '@users/services/auth.service';

export const authGuard: CanActivateFn = (
  next: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {
  if (inject(AuthService).isAuthenticated()) {
    return true;
  }

  console.warn('User is not authenticated');
  inject(Router).navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
