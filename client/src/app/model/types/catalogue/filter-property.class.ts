import { FilterPropertyDefinition } from "./filter-property-definition.class";
import { Product } from "./product.class";

export class FilterProperty {
  id: string;
  productId: string;
  filterPropertyDefinitionId: string;
  value: string;

  product?: Product;
  filterPropertyDefinition?: FilterPropertyDefinition;
}
