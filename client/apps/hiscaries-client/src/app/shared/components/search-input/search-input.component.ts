import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  Output,
  ViewChild,
} from '@angular/core';
import { DestroyService } from '../../services/destroy.service';

@Component({
  selector: 'app-search-input',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './search-input.component.html',
  styleUrls: ['./search-input.component.scss'],
  providers: [DestroyService],
})
export class SearchInputComponent implements AfterViewInit {
  private _defaultValue?: string | null;

  @ViewChild('searchValue') searchValueRef!: ElementRef;

  @Input() set defaultValue(value: string | null | undefined) {
    this._defaultValue = value;
    this.updateSearchInput();
  }

  get defaultValue(): string | null | undefined {
    return this._defaultValue;
  }

  @Output() searchAction = new EventEmitter<string>();

  isHighlighted = true;

  constructor() {
    setTimeout(() => (this.isHighlighted = false), 15000);
  }

  ngAfterViewInit(): void {
    this.updateSearchInput();
  }

  search(term: string): void {
    this.searchAction?.emit(term);
  }

  private updateSearchInput(): void {
    if (this.searchValueRef?.nativeElement && this.defaultValue) {
      this.searchValueRef.nativeElement.value = this.defaultValue;
    }
  }
}
