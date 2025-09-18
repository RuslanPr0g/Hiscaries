import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { DestroyService } from '../../services/destroy.service';

@Component({
  selector: 'app-progress-bar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './progress-bar.component.html',
  styleUrls: ['./progress-bar.component.scss'],
  providers: [DestroyService],
})
export class ProgressBarComponent {
  @Input() percentage = 0;
}
