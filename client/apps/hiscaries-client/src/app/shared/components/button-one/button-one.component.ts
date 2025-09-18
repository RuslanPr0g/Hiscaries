import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-button-one',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './button-one.component.html',
  styleUrls: ['./button-one.component.scss'],
})
export class ButtonOneComponent {
  @Input() label?: string;
  @Input() disabled?: boolean;
  @Output() clickAction = new EventEmitter<void>();

  click(): void {
    this.clickAction?.emit();
  }
}
