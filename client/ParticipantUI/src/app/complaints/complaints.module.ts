import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { SharedModule } from "../shared/shared.module";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { CommonModule } from "@angular/common";
import { ComplaintsHistoryComponent } from "./complaints-history/complaints-history.component";
import { ComplaintsComponent } from "./complaints/complaints.component";
import { ComplaintDetailsComponent } from "./complaint-details/complaint-details.component";

const routes: Routes = [
  {
    path: '',
    component: ComplaintsComponent
  },
  {
    path: 'history',
    component: ComplaintsHistoryComponent
  },
  {
    path: ':complaintId',
    component: ComplaintDetailsComponent
  },
];

@NgModule({
  declarations: [
    ComplaintsComponent,
    ComplaintsHistoryComponent,
    ComplaintDetailsComponent
  ],
  imports: [
    CommonModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    RouterModule.forChild(routes),
    SharedModule,
    RouterModule
  ],
  exports: [
    ComplaintsComponent,
    ComplaintsHistoryComponent,
    ComplaintDetailsComponent
  ]
})
export class ComplaintsModule { }
