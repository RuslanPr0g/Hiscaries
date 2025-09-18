import { Component, Input, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, ReactiveFormsModule } from '@angular/forms';
import { MessageModule } from 'primeng/message';
import { FileSelectEvent, FileUpload, FileUploadModule } from 'primeng/fileupload';

@Component({
  selector: 'app-upload-file-control',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FileUploadModule, MessageModule],
  templateUrl: './upload-file-control.component.html',
  styleUrls: ['./upload-file-control.component.scss'],
})
export class UploadFileControlComponent {
  @ViewChild('fileUpload') fileUpload!: FileUpload;

  @Input() control: AbstractControl<string | null, string | null> | null;
  @Input() centered = false;

  requiredErrorMessage = 'Image is required.';

  maxFileSize = 1048576;

  get hasImageSelected(): boolean {
    return !!this.control?.value;
  }

  onSelect(event: FileSelectEvent) {
    if (!this.control) {
      console.error('No control to upload a file was provided. Skipping file upload.');
      return;
    }

    const file = event.files[0];

    if (file.size > this.maxFileSize) {
      console.error(`File size exceeds the maximum limit of ${this.maxFileSize} bytes.`);
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      this.updateControlValue(reader);
    };
    reader.readAsDataURL(file);
  }

  clearSelection(): void {
    this.control?.setValue(null);
  }

  private updateControlValue(reader: FileReader): void {
    this.control?.patchValue(reader.result as string);
    this.fileUpload?.clear();
  }
}
