import { CommonModule } from '@angular/common';
import { Component, ViewChild, input } from '@angular/core';
import { AbstractControl, ReactiveFormsModule } from '@angular/forms';
import { Button } from 'primeng/button';
import { FileSelectEvent, FileUpload } from 'primeng/fileupload';
import { Message } from 'primeng/message';

export type UploadedFile = File;

@Component({
  selector: 'app-upload-file-control',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FileUpload, Message, Button],
  templateUrl: './upload-file-control.component.html',
  styleUrls: ['./upload-file-control.component.scss'],
})
export class UploadFileControlComponent {
  @ViewChild('fileUpload') fileUpload!: FileUpload;

  readonly control = input<AbstractControl<
    UploadedFile | string | null,
    UploadedFile | string | null
  > | null>();

  readonly fileType = input<'image' | 'pdf'>('image');

  readonly centered = input(false);

  readonly chooseLabel = input('Select file');

  requiredErrorMessage = 'File is required.';

  maxFileSize = 25 * 1024 * 1024; // 25 MB

  get hasFileSelected(): boolean {
    return !!this.control()?.value;
  }

  get accept(): string {
    return this.fileType() === 'image' ? 'image/*' : '.pdf';
  }

  private allowedMimeTypes(): string[] {
    return this.fileType() === 'image'
      ? ['image/png', 'image/jpeg', 'image/jpg', 'image/gif']
      : ['application/pdf'];
  }

  onSelect(event: FileSelectEvent) {
    if (!this.control()) {
      console.error('No control provided.');
      return;
    }

    const file = event.files[0];
    if (!file) return;

    if (file.size > this.maxFileSize) {
      console.error(`File exceeds ${this.maxFileSize} bytes.`);
      return;
    }

    if (!this.allowedMimeTypes().includes(file.type)) {
      console.error(`Invalid file type for ${this.fileType()}.`);
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      this.updateControlValue(reader);
    };
    reader.readAsDataURL(file);
  }

  clearSelection(): void {
    this.control()?.setValue(null);
  }

  private updateControlValue(reader: FileReader): void {
    this.control()?.patchValue(reader.result as string);
    this.fileUpload?.clear();
  }
}
