import { Component, input } from '@angular/core';
import { AbstractControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Editor } from 'primeng/editor';

@Component({
  selector: 'app-text-editor',
  standalone: true,
  imports: [Editor, ReactiveFormsModule, FormsModule],
  templateUrl: './text-editor.component.html',
  styleUrls: ['./text-editor.component.scss'],
})
export class TextEditorComponent {
  readonly control = input.required<AbstractControl<string>>();
  readonly errorMessage = input.required<string>();

  set text(value: string) {
    this.control().setValue(value);
  }

  get text(): string | null {
    return this.control()?.value;
  }
}
