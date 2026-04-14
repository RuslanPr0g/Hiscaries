import { SkeletonCardComponent } from './skeleton-card.component';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SkeletonModule } from 'primeng/skeleton';

describe('SkeletonCardComponent', () => {
  let fixture: ComponentFixture<SkeletonCardComponent>;

  /** All <p-skeleton> elements in the rendered DOM */
  function getAllSkeletons(): NodeListOf<HTMLElement> {
    return fixture.nativeElement.querySelectorAll('p-skeleton');
  }

  /** The first <p-skeleton> is always the image skeleton */
  function getImageSkeleton(): HTMLElement {
    return fixture.nativeElement.querySelector('p-skeleton');
  }

  /** Line skeletons are all <p-skeleton> elements after the first */
  function getLineSkeletons(): HTMLElement[] {
    const all = Array.from(getAllSkeletons());
    return all.slice(1);
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SkeletonCardComponent, SkeletonModule],
    }).compileComponents();

    fixture = TestBed.createComponent(SkeletonCardComponent);
    fixture.detectChanges();
  });

  // Requirements 2.1, 2.2 – correct number of skeleton lines rendered

  it('should render 2 skeleton lines by default', () => {
    expect(getLineSkeletons().length).toBe(2);
  });

  it('should render 1 skeleton line when lines=1', () => {
    fixture.componentRef.setInput('lines', 1);
    fixture.detectChanges();

    expect(getLineSkeletons().length).toBe(1);
  });

  it('should render 3 skeleton lines when lines=3', () => {
    fixture.componentRef.setInput('lines', 3);
    fixture.detectChanges();

    expect(getLineSkeletons().length).toBe(3);
  });

  it('should render 5 skeleton lines when lines=5', () => {
    fixture.componentRef.setInput('lines', 5);
    fixture.detectChanges();

    expect(getLineSkeletons().length).toBe(5);
  });

  it('should render 0 skeleton lines when lines=0', () => {
    fixture.componentRef.setInput('lines', 0);
    fixture.detectChanges();

    expect(getLineSkeletons().length).toBe(0);
  });

  // Requirements 2.3 – imageHeight is applied to the image skeleton element

  it('should always render exactly one image skeleton element', () => {
    expect(getImageSkeleton()).not.toBeNull();
  });

  it('should apply the default imageHeight of 170px to the image skeleton', () => {
    const imageSkeleton = getImageSkeleton();
    expect(imageSkeleton).not.toBeNull();
    // [style]="{ height: imageHeight() }" sets inline style on the p-skeleton host element
    expect(imageSkeleton.style.height).toBe('170px');
  });

  it('should apply a custom imageHeight to the image skeleton', () => {
    fixture.componentRef.setInput('imageHeight', '250px');
    fixture.detectChanges();

    const imageSkeleton = getImageSkeleton();
    expect(imageSkeleton).not.toBeNull();
    expect(imageSkeleton.style.height).toBe('250px');
  });
});
