export class TableColumnSettingsModel {
    public title: string;
    public dataPropName: string | undefined;
    public isOrderable: boolean;
    public isLink?: boolean = false;
    public pageLink?: string | null;
    public linkQueryParam?: string | null;
    public linkQueryDataPropName?: string | null;
    public isBoolean?: boolean = false;
}