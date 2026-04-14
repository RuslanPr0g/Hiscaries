import { BaseOptionModel } from '../../models/base-option.model';
import { CommonModule } from '@angular/common';
import { Component, input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Message } from 'primeng/message';
import { MultiSelect } from 'primeng/multiselect';

@Component({
  selector: 'app-form-multiselect',
  standalone: true,
  imports: [CommonModule, MultiSelect, ReactiveFormsModule, Message],
  templateUrl: './form-multiselect.component.html',
  styleUrls: ['./form-multiselect.component.scss'],
})
export class FormMultiselectComponent {
  readonly formGroup = input.required<FormGroup>();
  readonly controlName = input.required<string>();
  readonly centered = input(false);
  readonly items = input<BaseOptionModel[]>([]);

  errorMessage = 'At least one item should be selected from the list.';

  get placeholder(): string {
    return `Select ${this.controlName()}`;
  }
}
