import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { InputNumberModule } from 'primeng/inputnumber';
import { MessageModule } from 'primeng/message';

@Component({
  selector: 'app-number-limit-control',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, InputNumberModule, MessageModule],
  templateUrl: './number-limit-control.component.html',
  styleUrls: ['./number-limit-control.component.scss'],
})
export class NumberLimitControlComponent {
  @Input() formGroup!: FormGroup;
  @Input() controlName!: string;
  @Input() label!: string;
  @Input() errorMessage!: string;
  @Input() min = 0;
  @Input() max = 100;
  @Input() step = 1;
  @Input() centered = false;
}
