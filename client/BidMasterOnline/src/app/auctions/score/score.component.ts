import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ScoresService as ScoresService } from 'src/app/services/scores.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-score',
  templateUrl: './score.component.html'
})
export class ScoreComponent {
  @Input()
  auctionId: string;

  @Output()
  onScoreSet = new EventEmitter<void>()

  activeList: number[] = [];

  allScores = [1, 2, 3, 4, 5];

  constructor(private readonly scoresService: ScoresService,
    private readonly toastrService: ToastrService) {

  }

  onMouseHover(score: number) {
    this.activeList = [];

    for (let i = 1; i <= score; i++) {
      this.activeList.push(i);
    }
  }

  setScore(score: number) {
    this.scoresService.setScoreForAuction(this.auctionId, score).subscribe(
      (response) => {
        this.toastrService.success(response.message, 'Success');

        this.onScoreSet.emit();
      },
      (error) => {
        this.toastrService.error(error.error, 'Error');
      }
    );
  }
}
