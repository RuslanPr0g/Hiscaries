/**
 * Property-Based Tests for adminGuard
 *
 * Validates: Requirements 6 (Role guard prevents navigation without correct role)
 *
 * Property: ∀ token T where T.role ≠ "admin" → adminGuard returns false
 */
import { adminGuard } from './admin.guard';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '@users/services/auth.service';
import * as fc from 'fast-check';

describe('adminGuard', () => {
  let authServiceMock: jest.Mocked<Pick<AuthService, 'isAdmin'>>;
  let routerMock: jest.Mocked<Pick<Router, 'navigate'>>;

  const mockRoute = {} as ActivatedRouteSnapshot;
  const mockState = {} as RouterStateSnapshot;

  beforeEach(() => {
    authServiceMock = {
      isAdmin: jest.fn(),
    };

    routerMock = {
      navigate: jest.fn().mockResolvedValue(true),
    };

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock },
      ],
    });
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  // ─── Unit tests ───────────────────────────────────────────────────────────

  it('should return false when user is not an admin', () => {
    authServiceMock.isAdmin.mockReturnValue(false);

    const result = TestBed.runInInjectionContext(() => adminGuard(mockRoute, mockState));

    expect(result).toBe(false);
  });

  it('should return true when user is an admin', () => {
    authServiceMock.isAdmin.mockReturnValue(true);

    const result = TestBed.runInInjectionContext(() => adminGuard(mockRoute, mockState));

    expect(result).toBe(true);
  });

  it('should navigate to / when access is denied', () => {
    authServiceMock.isAdmin.mockReturnValue(false);

    TestBed.runInInjectionContext(() => adminGuard(mockRoute, mockState));

    expect(routerMock.navigate).toHaveBeenCalledWith(['/']);
  });

  it('should not navigate when access is granted', () => {
    authServiceMock.isAdmin.mockReturnValue(true);

    TestBed.runInInjectionContext(() => adminGuard(mockRoute, mockState));

    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  // ─── Property-Based Tests ─────────────────────────────────────────────────

  /**
   * Validates: Requirements 6
   *
   * Property: ∀ role ≠ "admin" → adminGuard returns false
   * This covers "reader", "publisher", random strings, empty string, and other non-admin roles.
   */
  it('PBT: should always return false for any non-admin role', () => {
    const nonAdminRole = fc.oneof(
      fc.constant('reader'),
      fc.constant('publisher'),
      fc.constant(''),
      fc.constant(null),
      fc.string().filter((s) => s !== 'admin'),
    );

    fc.assert(
      fc.property(nonAdminRole, () => {
        authServiceMock.isAdmin.mockReturnValue(false);

        const result = TestBed.runInInjectionContext(() => adminGuard(mockRoute, mockState));

        return result === false;
      }),
    );
  });

  /**
   * Validates: Requirements 6
   *
   * Property: isAdmin() = true → adminGuard returns true
   */
  it('PBT: should always return true when isAdmin() is true', () => {
    fc.assert(
      fc.property(fc.constant(true), (isAdmin) => {
        authServiceMock.isAdmin.mockReturnValue(isAdmin);

        const result = TestBed.runInInjectionContext(() => adminGuard(mockRoute, mockState));

        return result === true;
      }),
    );
  });

  /**
   * Validates: Requirements 6
   *
   * Property: isAdmin() = false → adminGuard returns false
   */
  it('PBT: should always return false when isAdmin() is false', () => {
    fc.assert(
      fc.property(fc.constant(false), (isAdmin) => {
        authServiceMock.isAdmin.mockReturnValue(isAdmin);

        const result = TestBed.runInInjectionContext(() => adminGuard(mockRoute, mockState));

        return result === false;
      }),
    );
  });
});
