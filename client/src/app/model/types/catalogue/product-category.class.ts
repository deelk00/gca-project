import { FilterPropertyDefinition } from './filter-property-definition.class';
import { Product } from './product.class';
import { Tag } from './tag.class';

export class ProductCategory {
  id: string;
  parentCategoryId?: string;
  name: string;

  tagIds: string[];
  childCategoryIds: string[];
  productIds: string[];
  filterPropertyDefinitionIds: string[];

  parentCategory?: ProductCategory;
  tags?: Tag[];
  childCategories?: ProductCategory[];
  products?: Product[];
  filterPropertyDefinitions?: FilterPropertyDefinition[];
}
