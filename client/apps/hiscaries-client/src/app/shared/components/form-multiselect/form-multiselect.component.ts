import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MessageModule } from 'primeng/message';
import { MultiSelectModule } from 'primeng/multiselect';
import { BaseOptionModel } from '../../models/base-option.model';

@Component({
  selector: 'app-form-multiselect',
  standalone: true,
  imports: [CommonModule, MultiSelectModule, ReactiveFormsModule, MessageModule],
  templateUrl: './form-multiselect.component.html',
  styleUrls: ['./form-multiselect.component.scss'],
})
export class FormMultiselectComponent {
  @Input() formGroup!: FormGroup;
  @Input() controlName!: string;
  @Input() centered = false;
  @Input() items: BaseOptionModel[] = [];

  errorMessage = 'At least one item should be selected from the list.';

  get placeholder(): string {
    return `Select ${this.controlName}`;
  }
}
