import {TypeDef} from "../type-def.abstract";
import {Tag} from "../../types/catalogue/tag.class";
import {IdListDescriptor} from "../id-list-descriptor.class";
import {IdDescriptor} from "../id-descriptor.class";
import {ListTypeDef} from "../list-type-def.class";
import {ProductTypeDef} from "./product-type-def.class";
import {ProductCategoryTypeDef} from "./product-category-type-def.class";

export class TagTypeDef extends TypeDef<Tag> {
  ctor: { new(): Tag } = Tag;
  service: string = "catalogue";
  route = "tags";

  id = new IdDescriptor("string");
  name = "";
  productIds = new IdListDescriptor("string", "products");
  productCategoryIds = new IdListDescriptor("string", "productCategories");

  products = new ListTypeDef("catalogue", ProductTypeDef, "products");
  productCategories = new ListTypeDef("catalogue", ProductCategoryTypeDef, "product-categories");
}
