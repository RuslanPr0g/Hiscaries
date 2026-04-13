import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  OnDestroy,
  effect,
  inject,
  input,
  output,
} from '@angular/core';

@Component({
  selector: 'ui-intersection-anchor',
  standalone: true,
  templateUrl: './intersection-anchor.component.html',
  styleUrl: './intersection-anchor.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    style: 'height: 0; display: block;',
  },
})
export class IntersectionAnchorComponent implements OnDestroy {
  readonly threshold = input(0);
  readonly disabled = input(false);
  readonly intersected = output<void>();

  private readonly el = inject(ElementRef);
  private observer: IntersectionObserver | undefined;

  constructor() {
    effect(() => {
      if (this.disabled()) {
        this.observer?.disconnect();
        this.observer = undefined;
      } else {
        this.setupObserver();
      }
    });
  }

  private setupObserver(): void {
    if (this.observer) return;
    if (typeof IntersectionObserver === 'undefined') {
      console.warn(
        'IntersectionObserver is not supported in this environment. ' +
          'IntersectionAnchorComponent will not emit intersected events.',
      );
      return;
    }
    this.observer = new IntersectionObserver(
      (entries) => {
        for (const entry of entries) {
          if (entry.isIntersecting && !this.disabled()) {
            this.intersected.emit();
          }
        }
      },
      { threshold: this.threshold() },
    );
    this.observer.observe(this.el.nativeElement);
  }

  ngOnDestroy(): void {
    this.observer?.disconnect();
  }
}
