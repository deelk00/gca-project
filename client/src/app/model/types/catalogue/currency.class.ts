import { Product } from "./product.class";

export class Currency {
  id: string;
  name: string;
  shortName: string;
  symbol: string;
  productIds: string[];

  products?: Product[];
}
