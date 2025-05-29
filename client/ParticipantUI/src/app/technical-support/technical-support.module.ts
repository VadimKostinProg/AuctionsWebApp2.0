import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule, Routes } from "@angular/router";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { TechnicalSupportComponent } from "./technical-support/technical-support.component";
import { SharedModule } from "../shared/shared.module";
import { SupportTicketsHistoryComponent } from "./support-tickets-history/support-tickets-history.component";
import { SupportTicketDetailsComponent } from "./support-ticket-details/support-ticket-details.component";

const routes: Routes = [
  {
    path: '',
    component: TechnicalSupportComponent
  },
  {
    path: 'support-tickets',
    component: SupportTicketsHistoryComponent
  },
  {
    path: 'support-tickets/:supportTicketId',
    component: SupportTicketDetailsComponent
  },
];

@NgModule({
  declarations: [
    TechnicalSupportComponent,
    SupportTicketsHistoryComponent,
    SupportTicketDetailsComponent
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
    TechnicalSupportComponent,
    SupportTicketsHistoryComponent,
    SupportTicketDetailsComponent
  ]
})
export class TechnicalSupportModule { }
