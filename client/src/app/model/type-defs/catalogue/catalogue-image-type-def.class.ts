import {TypeDef} from "../type-def.abstract";
import {CatalogueImage} from "../../types/catalogue/image.class";
import {IdDescriptor} from "../id-descriptor.class";
import {IdListDescriptor} from "../id-list-descriptor.class";
import {ListTypeDef} from "../list-type-def.class";
import {Brand} from "../../types/catalogue/brand.class";
import {BrandTypeDef} from "./brand-type-def.class";
import {ProductImageTypeDef} from "./product-image-type-def.class";
import {ProductImage} from "../../types/catalogue/product-image.class";

export class CatalogueImageTypeDef extends TypeDef<CatalogueImage> {
  ctor: { new(): CatalogueImage } = CatalogueImage;
  service: string = "catalogue";
  route = "images";

  id = new IdDescriptor<CatalogueImage>("guid");
  hash = "";
  source = "";
  brandIds = new IdListDescriptor<CatalogueImage>("guid", "brands");
  productImageIds = new IdListDescriptor<CatalogueImage>("guid", "productImages")

  brands = new ListTypeDef("catalogue", BrandTypeDef, "brands");
  productImages = new ListTypeDef(ProductImageTypeDef);
}
