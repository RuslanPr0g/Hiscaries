import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { DestroyService } from '../../services/destroy.service';
import { Observable } from 'rxjs';
import { NavigationConst } from '../../constants/navigation.const';
import { Router } from '@angular/router';
import { StoryStateModel } from '@stories/store/story-state.model';
import { Store } from '@ngrx/store';
import { searchSearchTerm } from '@stories/store/story.selector';
import { SearchInputComponent } from '../search-input/search-input.component';

@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [CommonModule, SearchInputComponent],
  templateUrl: './search-bar.component.html',
  styleUrls: ['./search-bar.component.scss'],
  providers: [DestroyService],
})
export class SearchBarComponent implements OnInit {
  searchTerm$: Observable<string | null>;

  constructor(private router: Router, private store: Store<StoryStateModel>) {
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
