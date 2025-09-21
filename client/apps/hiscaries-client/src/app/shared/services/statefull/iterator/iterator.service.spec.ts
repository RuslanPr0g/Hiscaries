import { TestBed } from '@angular/core/testing';
import { IteratorService } from './iterator.service';

describe('IteratorService', () => {
  let service: IteratorService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [IteratorService],
    });
    service = TestBed.inject(IteratorService);
  });

  it('should start with currentIndex 0', () => {
    expect(service.currentIndex).toBe(0);
  });

  it('should moveNext until upperBoundary', () => {
    service.upperBoundary = 3;
    expect(service.moveNext()).toBeTruthy();
    expect(service.currentIndex).toBe(1);
    expect(service.moveNext()).toBeTruthy();
    expect(service.currentIndex).toBe(2);
    expect(service.moveNext()).toBeTruthy();
    expect(service.currentIndex).toBe(3);
    expect(service.moveNext()).toBeFalsy();
    expect(service.currentIndex).toBe(3);
  });

  it('should movePrev until 0', () => {
    service.upperBoundary = 3;
    service.moveToLast();
    expect(service.movePrev()).toBeTruthy();
    expect(service.currentIndex).toBe(2);
    expect(service.movePrev()).toBeTruthy();
    expect(service.currentIndex).toBe(1);
    expect(service.movePrev()).toBeTruthy();
    expect(service.currentIndex).toBe(0);
    expect(service.movePrev()).toBeFalsy();
    expect(service.currentIndex).toBe(0);
  });

  it('moveTo should set index correctly', () => {
    service.upperBoundary = 5;
    expect(service.moveTo(2)).toBeTruthy();
    expect(service.currentIndex).toBe(2);
    expect(service.moveTo(5)).toBeTruthy();
    expect(service.currentIndex).toBe(5);
    expect(service.moveTo(0)).toBeTruthy();
    expect(service.currentIndex).toBe(0);
  });

  it('moveToLast should go to upperBoundary', () => {
    service.upperBoundary = 4;
    expect(service.moveToLast()).toBeTruthy();
    expect(service.currentIndex).toBe(4);
  });

  it('should handle upperBoundary set to 0', () => {
    service.upperBoundary = 0;
    expect(service.moveNext()).toBeFalsy();
    expect(service.currentIndex).toBe(0);
    expect(service.movePrev()).toBeFalsy();
    expect(service.moveToLast()).toBeTruthy();
    expect(service.currentIndex).toBe(0);
  });

  it('should allow multiple sequential moves', () => {
    service.upperBoundary = 2;
    expect(service.moveNext()).toBeTruthy();
    expect(service.moveNext()).toBeTruthy();
    expect(service.movePrev()).toBeTruthy();
    expect(service.moveNext()).toBeTruthy();
    expect(service.currentIndex).toBe(2);
  });
});
