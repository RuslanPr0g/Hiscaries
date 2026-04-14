import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'ui-section-header',
  standalone: true,
  imports: [],
  templateUrl: './section-header.component.html',
  styleUrl: './section-header.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SectionHeaderComponent {
  readonly title = input.required<string>();
  readonly level = input<1 | 2 | 3>(1);
}
