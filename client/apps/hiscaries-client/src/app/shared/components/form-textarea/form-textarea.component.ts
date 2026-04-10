import { Component, Input } from '@angular/core';

import { InputTextareaModule } from 'primeng/inputtextarea';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MessageModule } from 'primeng/message';

@Component({
  selector: 'app-form-textarea',
  standalone: true,
  imports: [InputTextareaModule, ReactiveFormsModule, MessageModule],
  templateUrl: './form-textarea.component.html',
  styleUrls: ['./form-textarea.component.scss'],
})
export class FormTextareaComponent {
  @Input() formGroup!: FormGroup;
  @Input() controlName!: string;
  @Input() label!: string;
  @Input() errorMessage = 'This field is required';
  @Input() rows = 4;
}
