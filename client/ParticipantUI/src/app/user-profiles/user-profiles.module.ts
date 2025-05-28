import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule, Routes } from "@angular/router";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { UserProfileComponent } from "./user-profile/user-profile.component";
import { UserFeedbacksComponent } from "./user-feedbacks/user-feedbacks.component";

const routes: Routes = [
  {
    path: ':userId',
    component: UserProfileComponent
  }
];

@NgModule({
  declarations: [
    UserProfileComponent,
    UserFeedbacksComponent
  ],
  imports: [
    CommonModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    RouterModule.forChild(routes),
    RouterModule
  ],
  exports: [
  ]
})
export class UserProfilesModule { }
