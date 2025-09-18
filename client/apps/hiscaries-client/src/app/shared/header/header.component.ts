import { Component, EventEmitter, Output } from '@angular/core';
import { AuthService } from '@users/services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { NavigationConst } from '../constants/navigation.const';
import { NotificationsBarComponent } from '../components/notifications-bar/notifications-bar.component';
import { SearchBarComponent } from '../components/search-bar/search-bar.component';

export interface MenuItem {
  Label: string;
  Command: () => void;
}

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, ButtonModule, NotificationsBarComponent, SearchBarComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  items: MenuItem[];

  @Output() commandExecuted = new EventEmitter<void>();

  constructor(public userService: AuthService, private router: Router) {
    this.items = [
      {
        Label: 'Home',
        Command: () => this.home(),
      },
    ];

    if (this.isUserPublisher) {
      this.items = [
        ...this.items,
        {
          Label: 'My Library',
          Command: () => this.navigateToMyLibrary(),
        },
        {
          Label: 'Publish story',
          Command: () => this.navigateToPublishStory(),
        },
      ];
    }

    this.items = [
      ...this.items,
      {
        Label: 'Reading history',
        Command: () => this.navigateToReadingHistory(),
      },
      {
        Label: 'Sign out',
        Command: () => this.logOut(),
      },
    ];
  }

  closeModal() {
    this.commandExecuted.emit();
  }

  get isUserPublisher(): boolean {
    return this.userService.isPublisher();
  }

  callItemCommand(item: MenuItem): void {
    item?.Command();
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
