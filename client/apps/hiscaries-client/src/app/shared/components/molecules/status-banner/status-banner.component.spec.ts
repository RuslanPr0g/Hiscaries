import { StatusBannerComponent } from './status-banner.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';

// Requirements 15.1–15.5

describe('StatusBannerComponent', () => {
  let fixture: ComponentFixture<StatusBannerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatusBannerComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(StatusBannerComponent);
    fixture.componentRef.setInput('message', 'Test message');
    fixture.detectChanges();
  });

  // Requirements 15.1 – message is rendered
  it('should render the message text', () => {
    const el: HTMLElement = fixture.nativeElement;
    expect(el.textContent).toContain('Test message');
  });

  // Requirements 15.2 – type 'info' produces banner-info class
  it('should apply banner-info class when type is info', () => {
    fixture.componentRef.setInput('type', 'info');
    fixture.detectChanges();

    const banner: HTMLElement = fixture.nativeElement.querySelector('.status-banner');
    expect(banner.classList).toContain('banner-info');
  });

  // Requirements 15.2 – type 'warning' produces banner-warning class
  it('should apply banner-warning class when type is warning', () => {
    fixture.componentRef.setInput('type', 'warning');
    fixture.detectChanges();

    const banner: HTMLElement = fixture.nativeElement.querySelector('.status-banner');
    expect(banner.classList).toContain('banner-warning');
  });

  // Requirements 15.2 – type 'error' produces banner-error class
  it('should apply banner-error class when type is error', () => {
    fixture.componentRef.setInput('type', 'error');
    fixture.detectChanges();

    const banner: HTMLElement = fixture.nativeElement.querySelector('.status-banner');
    expect(banner.classList).toContain('banner-error');
  });

  // Requirements 15.2 – type 'success' produces banner-success class
  it('should apply banner-success class when type is success', () => {
    fixture.componentRef.setInput('type', 'success');
    fixture.detectChanges();

    const banner: HTMLElement = fixture.nativeElement.querySelector('.status-banner');
    expect(banner.classList).toContain('banner-success');
  });

  // Requirements 15.2 – default type is 'info'
  it('should default to banner-info class when no type is provided', () => {
    const banner: HTMLElement = fixture.nativeElement.querySelector('.status-banner');
    expect(banner.classList).toContain('banner-info');
  });

  // Requirements 15.3 – icon rendered when non-null
  it('should render an icon element when icon is non-null', () => {
    fixture.componentRef.setInput('icon', 'pi pi-check');
    fixture.detectChanges();

    const icon: HTMLElement = fixture.nativeElement.querySelector('i');
    expect(icon).not.toBeNull();
    expect(icon.className).toContain('pi pi-check');
  });

  // Requirements 15.4 – icon absent when null
  it('should not render an icon element when icon is null', () => {
    fixture.componentRef.setInput('icon', null);
    fixture.detectChanges();

    const icon = fixture.nativeElement.querySelector('i');
    expect(icon).toBeNull();
  });
});
