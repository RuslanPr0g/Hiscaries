import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { HeaderComponent } from '@shared/header/header.component';
import { LoadingOverlayComponent } from '@shared/components/loading-overlay/loading-overlay.component';
import { CommonModule } from '@angular/common';
import { SidebarModule } from 'primeng/sidebar';
import { ButtonModule } from 'primeng/button';
import { NavigationConst } from '@shared/constants/navigation.const';
import { AuthService } from './users/services/auth.service';
import { NotificationLifecycleManagerService } from './users/services/notification-lifecycle-manager.service';
import { StoryPublishedHandler } from './users/notification-handlers/story-published-notification.handler';
import { NotificationStateService } from '@shared/services/statefull/notification-state.service';
import { Location } from '@angular/common';
import { filter, takeUntil } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    ButtonModule,
    SidebarModule,
    HeaderComponent,
    LoadingOverlayComponent,
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  title = 'hiscaries';

  loading = true;
  sidebarVisible = false;

  unreadCount = 0;

  constructor(
    private router: Router,
    public authService: AuthService,
    private notificationManagerService: NotificationLifecycleManagerService,
    private notificationStateService: NotificationStateService,
    private location: Location, // TODO: make it work
  ) // @Inject(NOTIFICATION_HANDLERS) private notificationHandlers: NotificationHandler[]
  {}

  ngOnInit() {
    setTimeout(() => {
      this.fadeOutLoading();

      if (this.authService.isAuthenticated()) {
        // TODO: fix DI
        this.notificationManagerService.initialize([new StoryPublishedHandler()]);
      }
    }, 1501);

    this.authService.loginEvent$.subscribe(() => {
      // TODO: fix DI
      this.notificationManagerService.initialize([new StoryPublishedHandler()]);
    });

    this.authService.logoutEvent$.subscribe(() => {
      this.notificationManagerService.stop();
    });

    this.notificationStateService.unreadCount$.subscribe((count) => {
      this.unreadCount = count;
    });

    this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
      this.sidebarVisible = false;
    });
  }

  goBack(): void {
    this.location.back();
  }

  fadeOutLoading() {
    this.loading = false;
  }

  home(): void {
    this.router.navigate([NavigationConst.Home]);
  }
}
