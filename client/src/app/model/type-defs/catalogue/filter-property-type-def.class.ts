import {TypeDef} from "../type-def.abstract";
import {FilterProperty} from "../../types/catalogue/filter-property.class";
import {IdDescriptor} from "../id-descriptor.class";
import {ProductTypeDef} from "./product-type-def.class";
import {FilterPropertyDefinitionTypeDef} from "./filter-property-definition-type-def.class";

export class FilterPropertyTypeDef extends TypeDef<FilterProperty> {
  ctor: { new(): FilterProperty } = FilterProperty;
  service: string = "catalogue";
  route = "filter-properties";

  id = new IdDescriptor("guid");
  productId = new IdDescriptor("guid", "product");
  filterPropertyDefinitionId = new IdDescriptor("guid", "filterPropertyDefinition");
  value = "";

  product = ProductTypeDef;
  filterPropertyDefinition = FilterPropertyDefinitionTypeDef;
}
