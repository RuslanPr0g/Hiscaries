import { Component, Input, Output, EventEmitter, HostBinding } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { PrimeNgIcon } from '@shared/types/primeng-icon.type';

@Component({
  selector: 'app-button-two',
  standalone: true,
  imports: [CommonModule, ButtonModule],
  templateUrl: './button-two.component.html',
  styleUrls: ['./button-two.component.scss'],
})
export class ButtonTwoComponent {
  @Input() label!: string;
  @Input() severity:
    | 'success'
    | 'info'
    | 'warning'
    | 'danger'
    | 'help'
    | 'primary'
    | 'secondary'
    | 'contrast'
    | null
    | undefined = null;
  @Input() disabled = false;
  @Input() iconType?: PrimeNgIcon;
  @Input() isFullWidth = false;
  @Output() clicked = new EventEmitter<void>();

  @HostBinding('style.width') get hostWidth() {
    return this.isFullWidth ? '100%' : 'auto';
  }

  handleClick() {
    this.clicked.emit();
  }
}
