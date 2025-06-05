import { CommonModule } from "@angular/common";
import { RouterModule, Routes } from "@angular/router";
import { SharedModule } from "../shared/shared.module";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { ToastrModule } from "ngx-toastr";
import { NgModule } from "@angular/core";
import { SupportTicketsListComponent } from "./support-tickets-list/support-tickets-list.component";
import { SupportTicketFiltersComponent } from "./support-ticket-filters/support-ticket-filters.component";
import { SupportTicketDetailsComponent } from "./support-ticket-details/support-ticket-details.component";

const routes: Routes = [
  { path: '', component: SupportTicketsListComponent },
  { path: ':supportTicketId', component: SupportTicketDetailsComponent },
];

@NgModule({
  declarations: [
    SupportTicketsListComponent,
    SupportTicketFiltersComponent,
    SupportTicketDetailsComponent
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
export class SupportTicketsManagementModule { }
