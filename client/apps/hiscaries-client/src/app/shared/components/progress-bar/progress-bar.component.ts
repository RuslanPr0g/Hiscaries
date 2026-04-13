
import { Component, input } from '@angular/core';
import { DestroyService } from '../../services/destroy.service';

@Component({
  selector: 'app-progress-bar',
  standalone: true,
  imports: [],
  templateUrl: './progress-bar.component.html',
  styleUrls: ['./progress-bar.component.scss'],
  providers: [DestroyService],
})
export class ProgressBarComponent {
  readonly percentage = input(0);
}
