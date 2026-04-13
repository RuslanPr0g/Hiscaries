import { CommonModule } from '@angular/common';
import { Component, inject, output, input } from '@angular/core';
import { ButtonTwoComponent } from '@shared/components/button-two/button-two.component';
import { FallbackImagePipe } from '@shared/pipes/fallback-image.pipe';
import { SocialMediaIconMapperService } from '@shared/services/social-media-icon-mapper.service';
import { LibraryModel } from '@users/models/domain/library.model';

@Component({
  selector: 'app-library-general-view',
  standalone: true,
  imports: [CommonModule, ButtonTwoComponent, FallbackImagePipe],
  templateUrl: './library-general-view.component.html',
  styleUrl: './library-general-view.component.scss',
})
export class LibraryGeneralViewComponent {
  private iconService = inject(SocialMediaIconMapperService);

  readonly library = input<LibraryModel>();
  readonly isAbleToEdit = input(false);

  readonly isAbleToSubscribe = input(false);
  readonly isSubscribed = input(false);

  readonly isSubscribeLoading = input(false);

  readonly editStarted = output<void>();

  readonly subscribed = output<void>();
  readonly unSubscribed = output<void>();

  getSocialNetworkIcon(link: string): string {
    return this.iconService.mapFromUrl(link);
  }

  startEdit(): void {
    // TODO: The 'emit' function requires a mandatory void argument
    this.editStarted?.emit();
  }

  get backgroundImageUrl(): string | undefined {
    const library = this.library();
    return (
      library?.AvatarImageUrls?.Large ??
      library?.AvatarImageUrls?.Medium ??
      library?.AvatarImageUrls?.Small
    );
  }

  subscribeAction(): void {
    // TODO: The 'emit' function requires a mandatory void argument
    this.subscribed?.emit();
  }

  unSubscribeAction(): void {
    // TODO: The 'emit' function requires a mandatory void argument
    this.unSubscribed?.emit();
  }
}
