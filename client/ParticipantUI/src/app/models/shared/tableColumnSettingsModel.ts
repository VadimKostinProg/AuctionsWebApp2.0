export class TableColumnSettingsModel {
  public title!: string;
  public dataPropName?: string | null;
  public isOrderable: boolean = false;
  public isLink: boolean = false;
  public pageLink?: string | null;
  public linkQueryParam?: string | null;
  public linkQueryDataPropName?: string | null;
  public isBoolean: boolean = false;
}
