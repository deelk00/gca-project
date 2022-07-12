import { CatalogueImage } from "./image.class";
import { Product } from "./product.class";

export class Brand {
  id: string;
  imageId: string;
  name: string;
  productIds: string[];

  get image(): CatalogueImage {

  }

  get products(): Product[] {

  }
}
