import { CommonModule } from "@angular/common";
import { DataTableComponent } from "./data-table/data-table.component";
import { SearchBarComponent } from "./search-bar/search-bar.component";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { NgSelectModule } from "@ng-select/ng-select";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { NgModule } from "@angular/core";

@NgModule({
  declarations: [
    SearchBarComponent,
    DataTableComponent,
  ],
  imports: [
    CommonModule,
    NgbModule,
    NgSelectModule,
    FormsModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    RouterModule
  ],
  exports: [
    SearchBarComponent,
    DataTableComponent,
  ]
})
export class SharedModule { }
