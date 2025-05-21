import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-score',
  standalone: false,
  templateUrl: './score.component.html'
})
export class ScoreComponent {
  @Output() onScoreSet = new EventEmitter<number>();

  activeList: number[] = [];

  allScores = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

  onMouseHover(score: number) {
    this.activeList = [];

    for (let i = 1; i <= score; i++) {
      this.activeList.push(i);
    }
  }

  setScore(score: number) {
    this.onScoreSet.emit(score);
  }
}
