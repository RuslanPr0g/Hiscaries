import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'ui-avatar-image',
  standalone: true,
  templateUrl: './avatar-image.component.html',
  styleUrl: './avatar-image.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AvatarImageComponent {
  readonly src = input<string | null>(null);
  readonly alt = input('Avatar');
  readonly size = input<'sm' | 'md' | 'lg'>('md');
  readonly initials = input<string | null>(null);
}
