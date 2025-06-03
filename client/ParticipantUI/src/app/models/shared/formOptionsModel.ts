import { FormGroup } from "@angular/forms";
import { FormPropertyModel } from "./formProperyModel";

export class FormOptionsModel {
  public form!: FormGroup;
  public properties!: FormPropertyModel[];
}
