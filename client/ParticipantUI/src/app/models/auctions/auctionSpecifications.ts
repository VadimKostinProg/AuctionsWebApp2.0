import { SortDirectionEnum } from "../sortDirectionEnum";
import { AuctionStatusEnum } from "./auctionStatusEnum";

export class AuctionSpecifications {
  public searchTerm?: string | null;
  public categoryId?: number | null;
  public typeId?: number | null;
  public minStartPrice?: number | null;
  public maxStartPrice?: number | null;
  public minCurrentPrice?: number | null;
  public maxCurrentPrice?: number | null;
  public auctionStatus?: AuctionStatusEnum | null;
  public sortBy?: string | null;
  public sortDirection?: SortDirectionEnum | null;
}
