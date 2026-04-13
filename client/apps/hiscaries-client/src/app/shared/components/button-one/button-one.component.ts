import { CommonModule } from '@angular/common';
import { Component, output, input } from '@angular/core';

@Component({
  selector: 'app-button-one',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './button-one.component.html',
  styleUrls: ['./button-one.component.scss'],
})
export class ButtonOneComponent {
  readonly label = input<string>();
  readonly disabled = input<boolean>();
  readonly clickAction = output<void>();

  click(): void {
    // TODO: The 'emit' function requires a mandatory void argument
    this.clickAction?.emit();
  }
}
