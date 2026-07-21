import { Component, inject } from '@angular/core';

import { LoadingService } from '../../../core/services/loading.service';

@Component({
  selector: 'app-loading',
  templateUrl: './loading.html',
  styleUrl: './loading.scss',
})
export class Loading {
  protected readonly loadingService = inject(LoadingService);
}
