import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { MessageModule } from 'primeng/message';
import { EditorModule } from 'primeng/editor';
import { TextEditorComponent } from '@shared/components/text-editor/text-editor.component';
import { ButtonTwoComponent } from '@shared/components/button-two/button-two.component';
import { ButtonModule } from 'primeng/button';
import { IteratorService } from '@shared/services/statefull/iterator/iterator.service';

@Component({
  selector: 'app-content-builder',
  standalone: true,
  imports: [
    CommonModule,
    EditorModule,
    ButtonModule,
    ReactiveFormsModule,
    MessageModule,
    FormsModule,
    TextEditorComponent,
    ButtonTwoComponent,
  ],
  providers: [IteratorService],
  templateUrl: './content-builder.component.html',
  styleUrls: ['./content-builder.component.scss'],
})
export class ContentBuilderComponent implements OnInit {
  private _contents: FormArray;

  pageInput = 1;

  @Input() formGroup: FormGroup;
  @Input() formArrayName: string;
  @Input() set contents(value: FormArray) {
    this._contents = value;
    this.setUpperBoundary();
    this.iterator.moveToFirst();
    this.pageInput = 1;
  }

  get contents(): FormArray {
    return this._contents;
  }

  constructor(private fb: FormBuilder, private iterator: IteratorService) {}

  ngOnInit(): void {
    if (this.contents.length === 0) {
      this.addContent();
    }

    this.setUpperBoundary();
  }

  get currentIndex(): number {
    return this.iterator.currentIndex;
  }

  get currentPageControl(): AbstractControl<string> {
    return this.contents.at(this.currentIndex);
  }

  moveNext(): boolean {
    const moved = this.iterator.moveNext();
    if (moved) this.pageInput = this.currentIndex + 1;
    return moved;
  }

  movePrev(): boolean {
    const moved = this.iterator.movePrev();
    if (moved) this.pageInput = this.currentIndex + 1;
    return moved;
  }

  goToPage() {
    const page = this.pageInput - 1;

    if (page >= 0 && page < this.contents.length) {
      this.iterator.moveTo(page);
    } else {
      this.pageInput = this.iterator.currentIndex + 1;
    }
  }

  addContent() {
    this.contents.push(this.fb.control(''));
    this.setUpperBoundary();
    this.iterator.moveToLast();
  }

  removeContent() {
    if (this.contents.length <= 1) {
      return;
    }

    this.contents.removeAt(this.currentIndex);
    this.setUpperBoundary();
    this.movePrev();
  }

  private setUpperBoundary(): void {
    this.iterator.upperBoundary = this.contents.length - 1;

    if (this.iterator.currentIndex > this.contents.length - 1) {
      this.iterator.moveToFirst();
    }
  }
}
