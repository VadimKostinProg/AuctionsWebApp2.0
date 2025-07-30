import { CommonModule } from "@angular/common";
import { RouterModule, Routes } from "@angular/router";
import { SharedModule } from "../shared/shared.module";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { ToastrModule } from "ngx-toastr";
import { NgModule } from "@angular/core";
import { ComplaintsListComponent } from "./complaints-list/complaints-list.component";
import { ComplaintFiltersComponent } from "./complaint-filters/complaint-filters.component";
import { ComplaintDetailsComponent } from "./complaint-details/complaint-details.component";

const routes: Routes = [
  { path: '', component: ComplaintsListComponent },
  { path: ':complaintId', component: ComplaintDetailsComponent },
];

@NgModule({
  declarations: [
    ComplaintsListComponent,
    ComplaintFiltersComponent,
    ComplaintDetailsComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    SharedModule,
    ReactiveFormsModule,
    FormsModule,
    NgbModule,
    ToastrModule.forRoot(),
  ],
  exports: [
  ]
})
export class ComplaintsManagementModule { }
