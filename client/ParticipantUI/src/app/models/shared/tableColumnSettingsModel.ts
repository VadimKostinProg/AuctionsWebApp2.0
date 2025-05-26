export class TableColumnSettingsModel {
  public title!: string;
  public dataPropName?: string | null;
  public isOrderable: boolean = false;
  public isLink: boolean = false;
  public pageLink?: string | null;
  public linkRouteParamName?: string | null;
  public isBoolean: boolean = false;
  public transformAction?: (prop: any) => any | null;
}
