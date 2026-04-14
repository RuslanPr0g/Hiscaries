import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

@Component({
  selector: 'ui-truncated-text',
  standalone: true,
  templateUrl: './truncated-text.component.html',
  styleUrl: './truncated-text.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TruncatedTextComponent {
  readonly text = input.required<string>();
  readonly maxLength = input(60);
  readonly suffix = input('...');
  readonly tag = input<'p' | 'h1' | 'h2' | 'h3' | 'h4' | 'span'>('span');

  protected readonly display = computed(() =>
    this.text().length <= this.maxLength()
      ? this.text()
      : this.text().slice(0, this.maxLength()) + this.suffix(),
  );
}
