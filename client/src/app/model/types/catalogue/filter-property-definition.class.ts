import { FilterPropertyValueType } from "../../enums/filter-property-value-type.enum";
import { FilterProperty } from "./filter-property.class";
import { ProductCategory } from "./product-category.class";

export class FilterPropertyDefinition {
  id: string;
  name: string;
  valueType: FilterPropertyValueType;
  filterPropertyIds: string[];
  productCategoryIds: string[];

  filterProperties?: FilterProperty[];
  productCategories?: ProductCategory[];
}
