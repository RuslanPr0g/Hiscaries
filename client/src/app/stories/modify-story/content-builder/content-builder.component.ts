import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MessageModule } from 'primeng/message';
import { EditorModule } from 'primeng/editor';
import { TextEditorComponent } from '../../../shared/components/text-editor/text-editor.component';
import { FormButtonComponent } from '../../../shared/components/form-button/form-button.component';
import { ButtonModule } from 'primeng/button';
import { IteratorService } from '../../../shared/services/iterator.service';

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
        FormButtonComponent,
    ],
    providers: [IteratorService],
    templateUrl: './content-builder.component.html',
    styleUrls: ['./content-builder.component.scss'],
})
export class ContentBuilderComponent implements OnInit {
    @Input() formGroup: FormGroup;
    @Input() formArrayName: string;
    @Input() contents: FormArray;

    constructor(
        private fb: FormBuilder,
        private iterator: IteratorService
    ) {}

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

    get currentPageLabel(): string {
        return `Page: ${this.currentIndex + 1} / ${this.contents.length}`;
    }

    moveNext(): boolean {
        return this.iterator.moveNext();
    }

    movePrev(): boolean {
        return this.iterator.movePrev();
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
        console.warn(this.contents.length - 1);
        this.iterator.upperBoundary = this.contents.length - 1;
    }
}
