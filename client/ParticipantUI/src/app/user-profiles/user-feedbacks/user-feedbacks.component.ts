import { Component, Input } from "@angular/core";

@Component({
  selector: 'app-user-feedbacks',
  templateUrl: './user-feedbacks.component.html',
  standalone: false
})
export class UserFeedbacksComponent {
  @Input() userId!: number;
}
