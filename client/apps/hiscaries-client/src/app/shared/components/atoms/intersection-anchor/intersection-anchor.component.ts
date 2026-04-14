import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
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
    style: 'height: 1px; display: block;',
  },
})
export class IntersectionAnchorComponent implements OnInit, OnDestroy {
  readonly disabled = input(false);
  readonly intersected = output<void>();

  private readonly el = inject(ElementRef);
  private observer: IntersectionObserver | undefined;

  ngOnInit(): void {
    if (typeof IntersectionObserver === 'undefined') return;

    this.observer = new IntersectionObserver(
      (entries) => {
        for (const entry of entries) {
          // Gate here — never disconnect/reconnect, just check disabled at emit time
          if (entry.isIntersecting && !this.disabled()) {
            this.intersected.emit();
          }
        }
      },
      { threshold: 0, rootMargin: '0px 0px 300px 0px' },
    );

    this.observer.observe(this.el.nativeElement);
  }

  ngOnDestroy(): void {
    this.observer?.disconnect();
  }
}
