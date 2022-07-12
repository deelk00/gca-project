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

  get parentCategory(): ProductCategory {}
  get tags(): Tag[] {}
  get childCategories(): ProductCategory[] {}
  get products(): Product[] {}
  get filterPropertyDefinitions(): FilterPropertyDefinition[] {}
}
