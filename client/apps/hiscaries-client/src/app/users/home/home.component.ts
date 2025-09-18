import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SearchStoryRecommendationsComponent } from '@stories/search-story-recommendations/search-story-recommendations.component';
import { SearchStoryResumeReadingComponent } from '@stories/search-story-resume-reading/search-story-resume-reading.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, SearchStoryRecommendationsComponent, SearchStoryResumeReadingComponent],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent {}
