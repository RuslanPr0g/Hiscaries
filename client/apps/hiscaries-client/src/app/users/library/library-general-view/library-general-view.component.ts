import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LibraryModel } from '@users/models/domain/library.model';
import { SocialMediaIconMapperService } from '@shared/services/social-media-icon-mapper.service';
import { FormButtonComponent } from '@shared/components/form-button/form-button.component';
import { FallbackImagePipe } from '@shared/pipes/fallback-image.pipe';

@Component({
  selector: 'app-library-general-view',
  standalone: true,
  imports: [CommonModule, FormButtonComponent, FallbackImagePipe],
  templateUrl: './library-general-view.component.html',
  styleUrl: './library-general-view.component.scss',
})
export class LibraryGeneralViewComponent {
  @Input() library: LibraryModel;
  @Input() isAbleToEdit = false;

  @Input() isAbleToSubscribe = false;
  @Input() isSubscribed = false;

  @Input() isSubscribeLoading = false;

  @Output() editStarted = new EventEmitter<void>();

  @Output() subscribed = new EventEmitter<void>();
  @Output() unSubscribed = new EventEmitter<void>();

  constructor(private iconService: SocialMediaIconMapperService) {}

  getSocialNetworkIcon(link: string): string {
    return this.iconService.mapFromUrl(link);
  }

  startEdit(): void {
    this.editStarted?.emit();
  }

  get backgroundImageUrl(): string | undefined {
    return this.library?.AvatarImageUrls?.Large ?? this.library?.AvatarImageUrls?.Medium ?? this.library?.AvatarImageUrls?.Small;
  }

  subscribeAction(): void {
    this.subscribed?.emit();
  }

  unSubscribeAction(): void {
    this.unSubscribed?.emit();
  }
}
