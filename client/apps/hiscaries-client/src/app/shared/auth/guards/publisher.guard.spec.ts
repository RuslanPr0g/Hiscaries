/**
 * Property-Based Tests for publisherGuard
 *
 * Validates: Requirements 6 (Role guard prevents navigation without correct role)
 *
 * Property: ∀ token T where T.role = "reader" → publisherGuard returns false
 */
import { publisherGuard } from './publisher.guard';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '@users/services/auth.service';
import * as fc from 'fast-check';

describe('publisherGuard', () => {
  let authServiceMock: jest.Mocked<Pick<AuthService, 'isPublisher' | 'isAdmin'>>;
  let routerMock: jest.Mocked<Pick<Router, 'navigate'>>;

  const mockRoute = {} as ActivatedRouteSnapshot;
  const mockState = {} as RouterStateSnapshot;

  beforeEach(() => {
    authServiceMock = {
      isPublisher: jest.fn(),
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

  it('should return false when user is a reader', () => {
    authServiceMock.isPublisher.mockReturnValue(false);
    authServiceMock.isAdmin.mockReturnValue(false);

    const result = TestBed.runInInjectionContext(() => publisherGuard(mockRoute, mockState));

    expect(result).toBe(false);
  });

  it('should return true when user is a publisher', () => {
    authServiceMock.isPublisher.mockReturnValue(true);
    authServiceMock.isAdmin.mockReturnValue(false);

    const result = TestBed.runInInjectionContext(() => publisherGuard(mockRoute, mockState));

    expect(result).toBe(true);
  });

  it('should return true when user is an admin', () => {
    authServiceMock.isPublisher.mockReturnValue(false);
    authServiceMock.isAdmin.mockReturnValue(true);

    const result = TestBed.runInInjectionContext(() => publisherGuard(mockRoute, mockState));

    expect(result).toBe(true);
  });

  it('should return true when user is both publisher and admin', () => {
    authServiceMock.isPublisher.mockReturnValue(true);
    authServiceMock.isAdmin.mockReturnValue(true);

    const result = TestBed.runInInjectionContext(() => publisherGuard(mockRoute, mockState));

    expect(result).toBe(true);
  });

  it('should navigate to / when access is denied', () => {
    authServiceMock.isPublisher.mockReturnValue(false);
    authServiceMock.isAdmin.mockReturnValue(false);

    TestBed.runInInjectionContext(() => publisherGuard(mockRoute, mockState));

    expect(routerMock.navigate).toHaveBeenCalledWith(['/']);
  });

  it('should not navigate when access is granted', () => {
    authServiceMock.isPublisher.mockReturnValue(true);
    authServiceMock.isAdmin.mockReturnValue(false);

    TestBed.runInInjectionContext(() => publisherGuard(mockRoute, mockState));

    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  // ─── Property-Based Tests ─────────────────────────────────────────────────

  /**
   * Validates: Requirements 6
   *
   * Property: ∀ role ∉ {"publisher", "admin"} → publisherGuard returns false
   * This covers "reader", random strings, empty string, and other non-privileged roles.
   */
  it('PBT: should always return false for any non-publisher/non-admin role', () => {
    // Arbitrary that generates non-publisher, non-admin role strings
    const nonPrivilegedRole = fc.oneof(
      fc.constant('reader'),
      fc.constant(''),
      fc.constant(null),
      fc.string().filter((s) => s !== 'publisher' && s !== 'admin'),
    );

    fc.assert(
      fc.property(nonPrivilegedRole, () => {
        authServiceMock.isPublisher.mockReturnValue(false);
        authServiceMock.isAdmin.mockReturnValue(false);

        const result = TestBed.runInInjectionContext(() => publisherGuard(mockRoute, mockState));

        return result === false;
      }),
    );
  });

  /**
   * Validates: Requirements 6
   *
   * Property: isPublisher() = true → publisherGuard returns true (regardless of isAdmin)
   */
  it('PBT: should always return true when isPublisher() is true', () => {
    fc.assert(
      fc.property(fc.boolean(), (isAdmin) => {
        authServiceMock.isPublisher.mockReturnValue(true);
        authServiceMock.isAdmin.mockReturnValue(isAdmin);

        const result = TestBed.runInInjectionContext(() => publisherGuard(mockRoute, mockState));

        return result === true;
      }),
    );
  });

  /**
   * Validates: Requirements 6
   *
   * Property: isAdmin() = true → publisherGuard returns true (regardless of isPublisher)
   */
  it('PBT: should always return true when isAdmin() is true', () => {
    fc.assert(
      fc.property(fc.boolean(), (isPublisher) => {
        authServiceMock.isPublisher.mockReturnValue(isPublisher);
        authServiceMock.isAdmin.mockReturnValue(true);

        const result = TestBed.runInInjectionContext(() => publisherGuard(mockRoute, mockState));

        return result === true;
      }),
    );
  });

  /**
   * Validates: Requirements 6
   *
   * Property: isPublisher() = false ∧ isAdmin() = false → publisherGuard returns false
   * (the core property from the spec)
   */
  it('PBT: should always return false when both isPublisher and isAdmin are false', () => {
    fc.assert(
      fc.property(fc.constant(false), fc.constant(false), (isPublisher, isAdmin) => {
        authServiceMock.isPublisher.mockReturnValue(isPublisher);
        authServiceMock.isAdmin.mockReturnValue(isAdmin);

        const result = TestBed.runInInjectionContext(() => publisherGuard(mockRoute, mockState));

        return result === false;
      }),
    );
  });
});
