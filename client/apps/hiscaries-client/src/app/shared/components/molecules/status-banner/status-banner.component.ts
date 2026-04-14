import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'ui-status-banner',
  standalone: true,
  imports: [],
  templateUrl: './status-banner.component.html',
  styleUrl: './status-banner.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StatusBannerComponent {
  readonly message = input.required<string>();
  readonly type = input<'info' | 'warning' | 'error' | 'success'>('info');
  readonly icon = input<string | null>(null);
}
