import { ErrorStateComponent } from './error-state.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';

describe('ErrorStateComponent', () => {
  let fixture: ComponentFixture<ErrorStateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ErrorStateComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ErrorStateComponent);
    fixture.componentRef.setInput('title', 'Something went wrong');
    fixture.detectChanges();
  });

  // Requirements 7.1 – title is rendered inside the error panel
  it('should render the title inside the error panel', () => {
    fixture.componentRef.setInput('title', 'Failed to load data');
    fixture.detectChanges();

    const panel: HTMLElement = fixture.nativeElement.querySelector('.error-state');
    const title: HTMLElement | null = fixture.nativeElement.querySelector('.error-state__title');
    expect(panel).not.toBeNull();
    expect(title).not.toBeNull();
    expect(title!.textContent?.trim()).toBe('Failed to load data');
  });

  // Requirements 7.2 – description rendered when non-null
  it('should render the description when it is provided', () => {
    fixture.componentRef.setInput('description', 'Please try again later.');
    fixture.detectChanges();

    const desc: HTMLElement | null = fixture.nativeElement.querySelector(
      '.error-state__description',
    );
    expect(desc).not.toBeNull();
    expect(desc!.textContent?.trim()).toBe('Please try again later.');
  });

  // Requirements 7.3 – description absent when null
  it('should not render the description when it is null', () => {
    fixture.componentRef.setInput('description', null);
    fixture.detectChanges();

    const desc: HTMLElement | null = fixture.nativeElement.querySelector(
      '.error-state__description',
    );
    expect(desc).toBeNull();
  });
});
