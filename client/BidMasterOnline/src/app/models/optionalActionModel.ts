import { FormGroup } from "@angular/forms";
import { FormPropertyModel } from "./formPropertyModel";

export class OptionalActionModel {
    public actionName: string;
    public form: FormGroup;
    public properties: FormPropertyModel[];
    public message: string;
}