import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SuspiciousActivityReportsListComponent } from "./suspicious-activity-reports-list/suspicious-activity-reports-list.component";
import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

const routes: Routes = [
  { path: '', component: SuspiciousActivityReportsListComponent },
];

@NgModule({
  declarations: [
    SuspiciousActivityReportsListComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule.forChild(routes),
  ],
  providers: [
  ]
})
export class SuspiciousActivityReportsModule { }
