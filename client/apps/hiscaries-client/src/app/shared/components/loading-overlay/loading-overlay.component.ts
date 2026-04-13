import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-loading-overlay',
  standalone: true,
  imports: [],
  templateUrl: './loading-overlay.component.html',
  styleUrls: ['./loading-overlay.component.scss'],
})
export class LoadingOverlayComponent implements OnInit {
  hide = false;

  ngOnInit() {
    setTimeout(() => {
      this.hide = true;
    }, 100);
  }
}
