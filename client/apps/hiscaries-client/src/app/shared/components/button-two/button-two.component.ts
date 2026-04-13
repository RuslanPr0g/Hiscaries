import { Component, HostBinding, output, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Button } from 'primeng/button';
import { PrimeNgIcon } from '@shared/types/primeng-icon.type';

@Component({
  selector: 'app-button-two',
  standalone: true,
  imports: [CommonModule, Button],
  templateUrl: './button-two.component.html',
  styleUrls: ['./button-two.component.scss'],
})
export class ButtonTwoComponent {
  readonly label = input.required<string>();
  readonly severity = input<'success' | 'info' | 'danger' | 'help' | 'primary' | 'secondary' | 'contrast' | null | undefined>(null);
  readonly disabled = input(false);
  readonly iconType = input<PrimeNgIcon>();
  readonly isFullWidth = input(false);
  readonly clicked = output<void>();

  @HostBinding('style.width') get hostWidth() {
    return this.isFullWidth() ? '100%' : 'auto';
  }

  handleClick() {
    // TODO: The 'emit' function requires a mandatory void argument
    this.clicked.emit();
  }
}
