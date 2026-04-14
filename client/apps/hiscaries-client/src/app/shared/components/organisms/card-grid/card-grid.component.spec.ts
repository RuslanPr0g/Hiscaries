import { CardGridComponent } from './card-grid.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';

// Requirements 16.1–16.7

describe('CardGridComponent', () => {
  let fixture: ComponentFixture<CardGridComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CardGridComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(CardGridComponent);
    fixture.detectChanges();
  });

  // Requirements 16.1 – skeleton shown when isLoading=true
  it('should render skeleton grid when isLoading is true', () => {
    fixture.componentRef.setInput('isLoading', true);
    fixture.detectChanges();

    const skeleton = fixture.nativeElement.querySelector('ui-skeleton-grid');
    const error = fixture.nativeElement.querySelector('ui-error-state');
    const empty = fixture.nativeElement.querySelector('ui-empty-state');
    const grid = fixture.nativeElement.querySelector('.card-grid');

    expect(skeleton).not.toBeNull();
    expect(error).toBeNull();
    expect(empty).toBeNull();
    expect(grid).toBeNull();
  });

  // Requirements 16.2 – error shown when errorTitle is non-null
  it('should render error state when errorTitle is non-null', () => {
    fixture.componentRef.setInput('isLoading', false);
    fixture.componentRef.setInput('errorTitle', 'Something went wrong');
    fixture.detectChanges();

    const skeleton = fixture.nativeElement.querySelector('ui-skeleton-grid');
    const error = fixture.nativeElement.querySelector('ui-error-state');
    const empty = fixture.nativeElement.querySelector('ui-empty-state');
    const grid = fixture.nativeElement.querySelector('.card-grid');

    expect(error).not.toBeNull();
    expect(skeleton).toBeNull();
    expect(empty).toBeNull();
    expect(grid).toBeNull();
  });

  // Requirements 16.3 – empty state shown when isEmpty=true, isLoading=false, errorTitle=null
  it('should render empty state when isEmpty=true, isLoading=false, errorTitle=null', () => {
    fixture.componentRef.setInput('isLoading', false);
    fixture.componentRef.setInput('errorTitle', null);
    fixture.componentRef.setInput('isEmpty', true);
    fixture.detectChanges();

    const skeleton = fixture.nativeElement.querySelector('ui-skeleton-grid');
    const error = fixture.nativeElement.querySelector('ui-error-state');
    const empty = fixture.nativeElement.querySelector('ui-empty-state');
    const grid = fixture.nativeElement.querySelector('.card-grid');

    expect(empty).not.toBeNull();
    expect(skeleton).toBeNull();
    expect(error).toBeNull();
    expect(grid).toBeNull();
  });

  // Requirements 16.4 – content shown when isLoading=false, errorTitle=null, isEmpty=false
  it('should render content grid when isLoading=false, errorTitle=null, isEmpty=false', () => {
    fixture.componentRef.setInput('isLoading', false);
    fixture.componentRef.setInput('errorTitle', null);
    fixture.componentRef.setInput('isEmpty', false);
    fixture.detectChanges();

    const skeleton = fixture.nativeElement.querySelector('ui-skeleton-grid');
    const error = fixture.nativeElement.querySelector('ui-error-state');
    const empty = fixture.nativeElement.querySelector('ui-empty-state');
    const grid = fixture.nativeElement.querySelector('.card-grid');

    expect(grid).not.toBeNull();
    expect(skeleton).toBeNull();
    expect(error).toBeNull();
    expect(empty).toBeNull();
  });

  // Requirements 16.5 – only one state rendered at a time
  it('should render only one state at a time — skeleton takes priority over error', () => {
    fixture.componentRef.setInput('isLoading', true);
    fixture.componentRef.setInput('errorTitle', 'Error');
    fixture.componentRef.setInput('isEmpty', true);
    fixture.detectChanges();

    const skeleton = fixture.nativeElement.querySelector('ui-skeleton-grid');
    const error = fixture.nativeElement.querySelector('ui-error-state');
    const empty = fixture.nativeElement.querySelector('ui-empty-state');
    const grid = fixture.nativeElement.querySelector('.card-grid');

    expect(skeleton).not.toBeNull();
    expect(error).toBeNull();
    expect(empty).toBeNull();
    expect(grid).toBeNull();
  });

  // Requirements 16.5 – error takes priority over empty
  it('should render only one state at a time — error takes priority over empty', () => {
    fixture.componentRef.setInput('isLoading', false);
    fixture.componentRef.setInput('errorTitle', 'Error');
    fixture.componentRef.setInput('isEmpty', true);
    fixture.detectChanges();

    const skeleton = fixture.nativeElement.querySelector('ui-skeleton-grid');
    const error = fixture.nativeElement.querySelector('ui-error-state');
    const empty = fixture.nativeElement.querySelector('ui-empty-state');
    const grid = fixture.nativeElement.querySelector('.card-grid');

    expect(error).not.toBeNull();
    expect(skeleton).toBeNull();
    expect(empty).toBeNull();
    expect(grid).toBeNull();
  });

  // Requirements 16.6 – gridCols uses minCardWidth
  it('should apply grid-template-columns using minCardWidth', () => {
    fixture.componentRef.setInput('isLoading', false);
    fixture.componentRef.setInput('errorTitle', null);
    fixture.componentRef.setInput('isEmpty', false);
    fixture.componentRef.setInput('minCardWidth', '250px');
    fixture.detectChanges();

    const grid = fixture.nativeElement.querySelector('.card-grid') as HTMLElement;
    expect(grid.style.gridTemplateColumns).toBe('repeat(auto-fill, minmax(250px, 1fr))');
  });
});
