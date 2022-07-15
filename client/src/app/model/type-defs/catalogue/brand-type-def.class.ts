import {TypeDef} from "../type-def.abstract";
import {Brand} from "../../types/catalogue/brand.class";
import {IdDescriptor} from "../id-descriptor.class";
import {CatalogueImageTypeDef} from "./catalogue-image-type-def.class";
import {ListTypeDef} from "../list-type-def.class";
import {Product} from "../../types/catalogue/product.class";
import {IdListDescriptor} from "../id-list-descriptor.class";
import {ProductTypeDef} from "./product-type-def.class";

export class BrandTypeDef extends TypeDef<Brand> {
  service = "catalogue";
  ctor = Brand;
  route = "brands";

  id: IdDescriptor<Brand> = new IdDescriptor<Brand>("string");
  name: string = "";
  imageId = new IdDescriptor<Brand>("string", "image");
  productIds = new IdListDescriptor<Brand>("string", "products");

  image = CatalogueImageTypeDef;
  products = new ListTypeDef("catalogue", ProductTypeDef, "products");
}
