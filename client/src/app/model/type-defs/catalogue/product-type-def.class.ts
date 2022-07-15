import {TypeDef} from "../type-def.abstract";
import {Product} from "../../types/catalogue/product.class";
import {IdDescriptor} from "../id-descriptor.class";
import {Gender} from "../../enums/gender.enum";
import {CurrencyTypeDef} from "./currency-type-def.class";
import {ProductCategoryTypeDef} from "./product-category-type-def.class";
import {IdListDescriptor} from "../id-list-descriptor.class";
import {ListTypeDef} from "../list-type-def.class";
import {Tag} from "../../types/catalogue/tag.class";
import {FilterPropertyTypeDef} from "./filter-property-type-def.class";
import {TagTypeDef} from "./tag-type-def.class";
import {BrandTypeDef} from "./brand-type-def.class";

export class ProductTypeDef extends TypeDef<Product> {
  ctor: { new(): Product } = Product;
  service: string = "catalogue";
  route = "products";

  id = new IdDescriptor("string");
  productCategoryId = new IdDescriptor("string", "productCategory");
  currencyId = new IdDescriptor("string", "currency");
  brandId = new IdDescriptor("string", "brand");
  name = "";
  stock = 0;
  gender = Gender.Uni;
  price = 0.0;

  currency = CurrencyTypeDef;
  productCategory = ProductCategoryTypeDef;
  tags = new ListTypeDef("catalogue", TagTypeDef, "tags");
  filterProperties = new ListTypeDef("catalogue", FilterPropertyTypeDef, "filter-properties");
  brand = BrandTypeDef;
}
