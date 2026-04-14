import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'ui-error-state',
  standalone: true,
  templateUrl: './error-state.component.html',
  styleUrl: './error-state.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ErrorStateComponent {
  readonly title = input.required<string>();
  readonly description = input<string | null>(null);
}
