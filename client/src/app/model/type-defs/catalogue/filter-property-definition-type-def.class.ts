import {TypeDef} from "../type-def.abstract";
import {FilterPropertyDefinition} from "../../types/catalogue/filter-property-definition.class";
import {IdDescriptor} from "../id-descriptor.class";
import {FilterPropertyValueType} from "../../enums/filter-property-value-type.enum";
import {IdListDescriptor} from "../id-list-descriptor.class";
import {ListTypeDef} from "../list-type-def.class";
import {FilterProperty} from "../../types/catalogue/filter-property.class";
import {ProductCategory} from "../../types/catalogue/product-category.class";
import {FilterPropertyTypeDef} from "./filter-property-type-def.class";
import {ProductCategoryTypeDef} from "./product-category-type-def.class";

export class FilterPropertyDefinitionTypeDef extends TypeDef<FilterPropertyDefinition> {
  ctor: { new(): FilterPropertyDefinition } = FilterPropertyDefinition;
  service: string = "catalogue";
  route = "filter-property-definitions";

  id = new IdDescriptor("string");
  name = "";
  valueType: FilterPropertyValueType = FilterPropertyValueType.Number;
  filterPropertyIds = new IdListDescriptor("string", "filterProperties");
  productCategoryIds = new IdListDescriptor("string", "productCategories");

  filterProperties = new ListTypeDef("catalogue", FilterPropertyTypeDef, "filter-properties");
  productCategories = new ListTypeDef("catalogue", ProductCategoryTypeDef, "product-categories");
}
