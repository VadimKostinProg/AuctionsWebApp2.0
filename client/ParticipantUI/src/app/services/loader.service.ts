import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {
  private loadingCount = 0;
  private _isLoading = new BehaviorSubject<boolean>(false);

  public readonly isLoading$: Observable<boolean> = this._isLoading.asObservable();

  constructor() { }

  show(): void {
    this.loadingCount++;
    if (this.loadingCount === 1) {
      this._isLoading.next(true);
    }
  }

  hide(): void {
    this.loadingCount--;
    if (this.loadingCount <= 0) {
      this.loadingCount = 0;
      this._isLoading.next(false);
    }
  }
}
