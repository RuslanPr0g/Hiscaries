import { StoryPublishedHandler } from './users/notification-handlers/story-published-notification.handler';
import { AuthService } from './users/services/auth.service';
import { NotificationLifecycleManagerService } from './users/services/notification-lifecycle-manager.service';
import { CommonModule } from '@angular/common';
import { Location } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { LoadingOverlayComponent } from '@shared/components/loading-overlay/loading-overlay.component';
import { NotificationsBarComponent } from '@shared/components/notifications-bar/notifications-bar.component';
import { NavigationConst } from '@shared/constants/navigation.const';
import { HeaderComponent } from '@shared/header/header.component';
import { DestroyService } from '@shared/services/destroy.service';
import { NotificationStateService } from '@shared/services/statefull/notification-state.service';
import { MessageService } from 'primeng/api';
import { Button } from 'primeng/button';
import { Toast } from 'primeng/toast';
import { filter, takeUntil } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    Button,
    HeaderComponent,
    LoadingOverlayComponent,
    Toast,
    NotificationsBarComponent,
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  providers: [MessageService, DestroyService],
})
export class AppComponent implements OnInit {
  private router = inject(Router);
  authService = inject(AuthService);
  private notificationManagerService = inject(NotificationLifecycleManagerService);
  private notificationStateService = inject(NotificationStateService);
  private location = inject(Location);

  title = 'hiscaries';

  loading = true;
  sidebarVisible = false;

  unreadCount = 0;

  notificationsVisible = false;

  newNotificationsAvailable = false;

  messageService = inject(MessageService);

  destroyService = inject(DestroyService);

  notificationService = inject(NotificationStateService);

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
