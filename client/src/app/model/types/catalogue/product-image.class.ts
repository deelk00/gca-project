import {CatalogueImage} from "./image.class";
import {Product} from "./product.class";

export class ProductImage {
  id: string;
  index: number;
  imageId: string;
  productId: string;

  image?: CatalogueImage;
  product?: Product;
}
