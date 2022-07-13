import {TypeDef} from "../type-def.abstract";
import {CatalogueImage} from "../../types/catalogue/image.class";
import {IdDescriptor} from "../id-descriptor.class";
import {IdListDescriptor} from "../id-list-descriptor.class";
import {ListTypeDef} from "../list-type-def.class";
import {Brand} from "../../types/catalogue/brand.class";
import {BrandTypeDef} from "./brand-type-def.class";

export class CatalogueImageTypeDef extends TypeDef<CatalogueImage> {
  ctor: { new(): CatalogueImage } = CatalogueImage;
  service: string = "catalogue";
  route = "images";

  id = new IdDescriptor<CatalogueImage>("string");
  hash = "";
  source = "";
  brandIds = new IdListDescriptor<CatalogueImage>("string", "brands");

  brands = new ListTypeDef("catalogue", BrandTypeDef, "brands");
}
