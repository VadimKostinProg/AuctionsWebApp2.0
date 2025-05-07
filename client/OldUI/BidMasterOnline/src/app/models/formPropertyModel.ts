import { FormInputTypeEnum } from "./formInputTypeEnum";
import { SelectOption } from "./selectOption";

export class FormPropertyModel {
    public label: string;
    public propName: string;
    public type: FormInputTypeEnum;
    public selectOptions: SelectOption[] | null;
}