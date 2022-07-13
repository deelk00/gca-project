import {TypeDef} from "../type-def.abstract";
import {Currency} from "../../types/catalogue/currency.class";
import {IdDescriptor} from "../id-descriptor.class";
import {ListTypeDef} from "../list-type-def.class";
import {Product} from "../../types/catalogue/product.class";
import {IdListDescriptor} from "../id-list-descriptor.class";
import {ProductTypeDef} from "./product-type-def.class";

export class CurrencyTypeDef extends TypeDef<Currency> {
  service = "catalogue";
  ctor: { new(): Currency } = Currency;
  route = "currencies";

  id = new IdDescriptor<Currency>("string");
  name = "";
  shortName = "";
  symbol = "";
  productIds = new IdListDescriptor("string", "products");

  products = new ListTypeDef("catalogue", ProductTypeDef, "products");
}
