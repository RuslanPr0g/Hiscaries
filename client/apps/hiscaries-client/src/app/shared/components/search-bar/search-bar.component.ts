import { NavigationConst } from '../../constants/navigation.const';
import { DestroyService } from '../../services/destroy.service';
import { SearchInputComponent } from '../search-input/search-input.component';
import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { StoryStateModel } from '@stories/store/story-state.model';
import { searchSearchTerm } from '@stories/store/story.selector';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [CommonModule, SearchInputComponent],
  templateUrl: './search-bar.component.html',
  styleUrls: ['./search-bar.component.scss'],
  providers: [DestroyService],
})
export class SearchBarComponent implements OnInit {
  private router = inject(Router);
  private store = inject<Store<StoryStateModel>>(Store);

  searchTerm$: Observable<string | null>;

  constructor() {
    this.searchTerm$ = this.store.select(searchSearchTerm);
  }

  ngOnInit(): void {
    // TODO: I do not fkcing know why it doesnt work, Im giving up
    setTimeout(
      () => this.searchTerm$.subscribe((value) => console.log('Searching story...', value)),
      3000,
    );
  }

  search(term: string): void {
    this.router.navigate([NavigationConst.SearchStory(term)]);
  }
}
