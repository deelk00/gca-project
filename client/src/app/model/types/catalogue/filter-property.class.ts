import { FilterPropertyDefinition } from "./filter-property-definition.class";
import { Product } from "./product.class";

export class FilterProperty {
  id: string;
  productId: string;
  filterPropertyDefinitionId: string;
  value: string;

  get product(): Product {}
  get filterPropertyDefinition(): FilterPropertyDefinition {

  }
}
