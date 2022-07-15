import {TypeDef} from "../type-def.abstract";
import {ProductCategory} from "../../types/catalogue/product-category.class";
import {IdDescriptor} from "../id-descriptor.class";
import {IdListDescriptor} from "../id-list-descriptor.class";
import {ListTypeDef} from "../list-type-def.class";
import {Tag} from "../../types/catalogue/tag.class";
import {TagTypeDef} from "./tag-type-def.class";
import {ProductTypeDef} from "./product-type-def.class";
import {FilterPropertyDefinitionTypeDef} from "./filter-property-definition-type-def.class";

export class ProductCategoryTypeDef extends TypeDef<ProductCategory> {
  ctor: { new(): ProductCategory } = ProductCategory;
  service: string = "catalogue";
  route = "product-categories";

  id = new IdDescriptor("string");
  name = "";
  parentCategoryId = new IdDescriptor("string", "parentCategory");
  tagIds = new IdListDescriptor("string", "tags");
  childCategoryIds = new IdListDescriptor("string", "childCategories");
  productIds = new IdListDescriptor("string", "products");
  filterPropertyDefinitionIds = new IdListDescriptor("string", "filterPropertyDefinitions");

  parentCategory = ProductCategoryTypeDef;
  tags = new ListTypeDef("catalogue", TagTypeDef, "tags")
  childCategories = new ListTypeDef("catalogue", ProductCategoryTypeDef, "product-categories");
  products = new ListTypeDef("catalogue", ProductTypeDef, "products");
  filterPropertyDefinitions = new ListTypeDef("catalogue", FilterPropertyDefinitionTypeDef, "filter-property-definitions");
}
