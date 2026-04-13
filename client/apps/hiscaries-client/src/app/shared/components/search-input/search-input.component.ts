
import {
  AfterViewInit,
  Component,
  ElementRef,
  Input,
  ViewChild,
  output
} from '@angular/core';
import { DestroyService } from '../../services/destroy.service';

@Component({
  selector: 'app-search-input',
  standalone: true,
  imports: [],
  templateUrl: './search-input.component.html',
  styleUrls: ['./search-input.component.scss'],
  providers: [DestroyService],
})
export class SearchInputComponent implements AfterViewInit {
  private _defaultValue?: string | null;

  @ViewChild('searchValue') searchValueRef!: ElementRef;

  // TODO: Skipped for migration because:
  //  Accessor inputs cannot be migrated as they are too complex.
  @Input() set defaultValue(value: string | null | undefined) {
    this._defaultValue = value;
    this.updateSearchInput();
  }

  get defaultValue(): string | null | undefined {
    return this._defaultValue;
  }

  readonly searchAction = output<string>();

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
