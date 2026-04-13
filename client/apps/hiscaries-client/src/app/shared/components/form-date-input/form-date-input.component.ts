import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { DatePicker } from 'primeng/datepicker';
import { Message } from 'primeng/message';

@Component({
  selector: 'app-form-date-input',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DatePicker, Message],
  templateUrl: './form-date-input.component.html',
  styleUrls: ['./form-date-input.component.scss'],
})
export class FormDateInputComponent {
  readonly formGroup = input.required<FormGroup>();
  readonly controlName = input.required<string>();
  readonly label = input.required<string>();
  readonly errorMessage = input.required<string>();
  readonly centered = input(false);
}
