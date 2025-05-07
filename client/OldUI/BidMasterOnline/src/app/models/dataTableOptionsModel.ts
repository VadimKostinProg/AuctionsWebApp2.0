import { FormOptionsModel } from "./formOptionsModel";
import { OptionalActionModel } from "./optionalActionModel";
import { TableColumnSettingsModel } from "./tableColumnSettingsModel";

export class DataTableOptionsModel {
    public title: string;
    public resourceName: string;
    public showIndexColumn: boolean;
    public allowCreating: boolean;
    public createFormOptions: FormOptionsModel | null;
    public allowEdit: boolean;
    public editFormOptions: FormOptionsModel | null;
    public allowDelete: boolean;
    public optionalAction: OptionalActionModel;
    public emptyListDisplayLabel: string;
    public columnSettings: TableColumnSettingsModel[];
}