import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CalendarModule } from 'primeng/calendar';
import { MessageModule } from 'primeng/message';

@Component({
  selector: 'app-form-date-input',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CalendarModule, MessageModule],
  templateUrl: './form-date-input.component.html',
  styleUrls: ['./form-date-input.component.scss'],
})
export class FormDateInputComponent {
  @Input() formGroup!: FormGroup;
  @Input() controlName!: string;
  @Input() label!: string;
  @Input() errorMessage!: string;
  @Input() centered = false;
}
