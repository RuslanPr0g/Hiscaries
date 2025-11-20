import { Component, inject, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { HeaderComponent } from '@shared/header/header.component';
import { LoadingOverlayComponent } from '@shared/components/loading-overlay/loading-overlay.component';
import { CommonModule } from '@angular/common';
import { SidebarModule } from 'primeng/sidebar';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { NavigationConst } from '@shared/constants/navigation.const';
import { AuthService } from './users/services/auth.service';
import { NotificationLifecycleManagerService } from './users/services/notification-lifecycle-manager.service';
import { StoryPublishedHandler } from './users/notification-handlers/story-published-notification.handler';
import { NotificationStateService } from '@shared/services/statefull/notification-state.service';
import { Location } from '@angular/common';
import { filter, takeUntil } from 'rxjs';
import { MessageService } from 'primeng/api';
import { NotificationsBarComponent } from '@shared/components/notifications-bar/notifications-bar.component';
import { DestroyService } from '@shared/services/destroy.service';

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
    ToastModule,
    NotificationsBarComponent,
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  providers: [MessageService, DestroyService],
})
export class AppComponent implements OnInit {
  title = 'hiscaries';

  loading = true;
  sidebarVisible = false;

  unreadCount = 0;

  notificationsVisible = false;

  newNotificationsAvailable = false;

  messageService = inject(MessageService);

  destroyService = inject(DestroyService);

  notificationService = inject(NotificationStateService);

  constructor(
    private router: Router,
    public authService: AuthService,
    private notificationManagerService: NotificationLifecycleManagerService,
    private notificationStateService: NotificationStateService,
    private location: Location, // TODO: make it work // @Inject(NOTIFICATION_HANDLERS) private notificationHandlers: NotificationHandler[]
  ) {}

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
      this.onNotificationClose();
    });

    this.notificationStateService.notifications$
      .pipe(takeUntil(this.destroyService.subject$))
      .subscribe((notifications) => {
        this.showNewMessagesAvailable(notifications.some((n) => !n.IsRead));
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

  showNewMessagesAvailable(newNotificationsAvailable: boolean) {
    this.newNotificationsAvailable = newNotificationsAvailable;
  }

  showNotifications() {
    if (!this.notificationsVisible) {
      this.messageService.add({
        key: 'notifications',
        sticky: true,
        severity: 'custom',
        summary: 'Notifications',
        styleClass: 'wide-toast backdrop-blur-lg rounded-2xl',
      });
      this.notificationsVisible = true;
    }
  }

  onNotificationClose() {
    this.notificationsVisible = false;
    this.messageService.clear();
  }
}
