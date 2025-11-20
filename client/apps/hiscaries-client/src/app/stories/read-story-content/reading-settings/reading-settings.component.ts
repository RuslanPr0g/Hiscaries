import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { InputSwitchModule } from 'primeng/inputswitch';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  defaultReadingSettings,
  ReadingSettings,
} from '@stories/models/domain/reading-settings.model';
import { LoadReadingSettingsService } from '@stories/services/load-reading-settings.service';

@Component({
  selector: 'app-reading-settings',
  standalone: true,
  imports: [CommonModule, InputSwitchModule, FormsModule],
  templateUrl: './reading-settings.component.html',
  styleUrls: ['./reading-settings.component.scss'],
})
export class ReadingSettingsComponent implements OnInit {
  @Output() settingsChanged = new EventEmitter<ReadingSettings>();

  settings: ReadingSettings = defaultReadingSettings;

  settingsService = inject(LoadReadingSettingsService);

  ngOnInit(): void {
    this.settings = this.settingsService.getSettings();
    this.settingsChanged.emit(this.settings);
  }

  toggleTheme(): void {
    this.settingsService.setSettings(this.settings);
    this.settingsChanged.emit(this.settings);
  }
}
