import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationStateService } from '../../services/statefull/notification-state.service';
import { NotificationModel } from '../../models/notification.model';
import { DestroyService } from '../../services/destroy.service';
import { takeUntil } from 'rxjs';
import { UserNotificationTypes } from '../../constants/notification-type.const';
import { NavigationConst } from '../../constants/navigation.const';
import { Router } from '@angular/router';
import { FallbackImagePipe } from '@shared/pipes/fallback-image.pipe';

@Component({
  selector: 'app-notifications-bar',
  standalone: true,
  imports: [CommonModule, FallbackImagePipe],
  templateUrl: './notifications-bar.component.html',
  styleUrls: ['./notifications-bar.component.scss'],
  providers: [DestroyService],
})
export class NotificationsBarComponent implements OnInit {
  notifications: NotificationModel[] = [];

  @Output() notificationClicked = new EventEmitter();

  constructor(
    private router: Router,
    private notificationStateService: NotificationStateService,
    private destroyService: DestroyService,
  ) {}

  ngOnInit(): void {
    this.notificationStateService.notifications$
      .pipe(takeUntil(this.destroyService.subject$))
      .subscribe((notifications) => {
        this.notifications = notifications;
      });
  }

  markAllAsRead(): void {
    const unReadNotifications = this.notifications.filter((x) => !x.IsRead);
    this.notificationStateService.markNotificationsAsRead(unReadNotifications);
  }

  clickNotification(notification: NotificationModel): void {
    const action = this.getActionByNotificationType(notification);

    if (action && notification.RelatedObjectId) {
      action(notification.RelatedObjectId);
    }

    this.notificationClicked.emit();
  }

  private getActionByNotificationType(
    notification: NotificationModel,
  ): (relatedObjectId: string) => void {
    if (
      notification.Type === UserNotificationTypes.StoryPublished &&
      notification.RelatedObjectId
    ) {
      return (storyId: string) => {
        this.router.navigate([NavigationConst.PreviewStory(storyId)]);
      };
    }

    return (relatedObjectId: string) => {
      console.log(
        `${notification.Type} notification doesn't have any action to be performed.
                 ${relatedObjectId} is the related object id.`,
      );
    };
  }
}
