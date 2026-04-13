import {
    AfterViewInit,
    ChangeDetectionStrategy,
    Component,
    ElementRef,
    OnDestroy,
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
export class IntersectionAnchorComponent implements AfterViewInit, OnDestroy {
    readonly threshold = input(0);
    readonly disabled = input(false);
    readonly intersected = output<void>();

    private readonly el = inject(ElementRef);
    private observer: IntersectionObserver | undefined;

    ngAfterViewInit(): void {
        if (this.disabled()) {
            return;
        }

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
