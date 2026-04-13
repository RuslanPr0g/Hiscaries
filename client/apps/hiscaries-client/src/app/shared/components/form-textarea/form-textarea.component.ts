import { Component, input } from '@angular/core';

import { Textarea } from 'primeng/textarea';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Message } from 'primeng/message';

@Component({
  selector: 'app-form-textarea',
  standalone: true,
  imports: [Textarea, ReactiveFormsModule, Message],
  templateUrl: './form-textarea.component.html',
  styleUrls: ['./form-textarea.component.scss'],
})
export class FormTextareaComponent {
  readonly formGroup = input.required<FormGroup>();
  readonly controlName = input.required<string>();
  readonly label = input.required<string>();
  readonly errorMessage = input('This field is required');
  readonly rows = input(4);
}
