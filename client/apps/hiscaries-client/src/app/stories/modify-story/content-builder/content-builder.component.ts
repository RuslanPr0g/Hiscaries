import { Component, Input, OnInit, inject, input } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { ButtonTwoComponent } from '@shared/components/button-two/button-two.component';
import { TextEditorComponent } from '@shared/components/text-editor/text-editor.component';
import { IteratorService } from '@shared/services/statefull/iterator/iterator.service';
import { Button } from 'primeng/button';

@Component({
  selector: 'app-content-builder',
  standalone: true,
  imports: [Button, ReactiveFormsModule, FormsModule, TextEditorComponent, ButtonTwoComponent],
  providers: [IteratorService],
  templateUrl: './content-builder.component.html',
  styleUrls: ['./content-builder.component.scss'],
})
export class ContentBuilderComponent implements OnInit {
  private fb = inject(FormBuilder);
  private iterator = inject(IteratorService);

  private _contents: FormArray;

  pageInput = 1;

  readonly formGroup = input<FormGroup>();
  readonly formArrayName = input<string>();
  // TODO: Skipped for migration because:
  //  Accessor inputs cannot be migrated as they are too complex.
  @Input() set contents(value: FormArray) {
    this._contents = value;
    this.setUpperBoundary();
    this.iterator.moveToFirst();
    this.pageInput = 1;
  }

  get contents(): FormArray {
    return this._contents;
  }

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
