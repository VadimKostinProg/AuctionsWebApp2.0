import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-score',
  standalone: false,
  templateUrl: './score.component.html'
})
export class ScoreComponent implements OnInit {
  @Input() selectedScore: number = 0;
  @Input() readonly: boolean = false;
  @Output() onScoreSet = new EventEmitter<number>();

  activeList: number[] = [];

  allScores = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

  ngOnInit(): void {
    if (this.readonly) {
      for (let i = 1; i <= this.selectedScore; i++) {
        this.activeList.push(i);
      }
    }
  }

  onMouseHover(score: number) {
    this.activeList = [];

    for (let i = 1; i <= score; i++) {
      this.activeList.push(i);
    }
  }

  onMouseLeave() {
    this.activeList = [];

    for (let i = 1; i <= this.selectedScore; i++) {
      this.activeList.push(i);
    }
  }

  setScore(score: number) {
    this.selectedScore = score;

    this.onScoreSet.emit(score);
  }
}
