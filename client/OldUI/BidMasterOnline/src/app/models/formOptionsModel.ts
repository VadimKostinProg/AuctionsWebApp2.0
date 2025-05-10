import { FormGroup } from "@angular/forms";
import { FormPropertyModel } from "./formPropertyModel";

export class FormOptionsModel {
    public form: FormGroup;
    public properties: FormPropertyModel[];
}