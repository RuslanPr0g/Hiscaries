import { CommonModule } from '@angular/common';
import { Component, input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { InputNumber } from 'primeng/inputnumber';
import { Message } from 'primeng/message';

@Component({
  selector: 'app-number-limit-control',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, InputNumber, Message],
  templateUrl: './number-limit-control.component.html',
  styleUrls: ['./number-limit-control.component.scss'],
})
export class NumberLimitControlComponent {
  readonly formGroup = input.required<FormGroup>();
  readonly controlName = input.required<string>();
  readonly label = input.required<string>();
  readonly errorMessage = input.required<string>();
  readonly min = input(0);
  readonly max = input(100);
  readonly step = input(1);
  readonly centered = input(false);
}
