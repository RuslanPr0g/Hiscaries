import { SearchBarComponent } from '../components/search-bar/search-bar.component';
import { NavigationConst } from '../constants/navigation.const';
import { CommonModule } from '@angular/common';
import { Component, HostListener, inject, output } from '@angular/core';
import { Router } from '@angular/router';
import { ButtonTwoComponent } from '@shared/components/button-two/button-two.component';
import { PrimeNgIcon } from '@shared/types/primeng-icon.type';
import { AuthService } from '@users/services/auth.service';

export interface MenuItem {
  Label: string;
  Command: () => void;
  Icon: PrimeNgIcon;
}

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, SearchBarComponent, ButtonTwoComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  userService = inject(AuthService);
  private router = inject(Router);

  items: MenuItem[];

  readonly commandExecuted = output<void>();

  constructor() {
    this.items = [];

    if (this.isUserPublisher) {
      this.items = [
        ...this.items,
        {
          Label: 'My Library',
          Command: () => this.navigateToMyLibrary(),
          Icon: 'pi-book',
        },
        {
          Label: 'Publish story',
          Command: () => this.navigateToPublishStory(),
          Icon: 'pi-upload',
        },
      ];
    }

    this.items = [
      ...this.items,
      {
        Label: 'History',
        Command: () => this.navigateToReadingHistory(),
        Icon: 'pi-history',
      },
      {
        Label: 'Sign out',
        Command: () => this.logOut(),
        Icon: 'pi-sign-out',
      },
    ];
  }

  @HostListener('document:keydown.escape', ['$event'])
  onEscapePress(): void {
    this.closeModal();
  }

  closeModal() {
    // TODO: The 'emit' function requires a mandatory void argument
    this.commandExecuted.emit();
  }

  get isUserPublisher(): boolean {
    return this.canPublish();
  }

  canPublish(): boolean {
    return this.userService.isPublisher() || this.userService.isAdmin();
  }

  callItemCommand(item: MenuItem): void {
    item?.Command();
    // TODO: The 'emit' function requires a mandatory void argument
    this.commandExecuted?.emit();
  }

  home(): void {
    this.router.navigate([NavigationConst.Home]);
  }

  logOut(): void {
    this.userService.logOut();
    this.router.navigate([NavigationConst.Login]);
  }

  navigateToPublishStory(): void {
    this.router.navigate([NavigationConst.PublishStory]);
  }

  navigateToMyLibrary(): void {
    this.router.navigate([NavigationConst.MyLibrary]);
  }

  navigateToReadingHistory(): void {
    this.router.navigate([NavigationConst.ReadingHistory]);
  }

  navigateToBecomePublisherPage(): void {
    this.router.navigate([NavigationConst.BecomePublisher]);
  }
}
