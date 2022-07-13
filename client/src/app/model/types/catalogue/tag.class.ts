import { Product } from './product.class';
import { ProductCategory } from './product-category.class';
export class Tag {
  id: string;
  name: string;

  productIds: string[];
  productCategoryIds: string[];

  products?: Product[];
  productCategories?: ProductCategory[];
}
