import {TypeDef} from "../type-def.abstract";
import {ProductCategory} from "../../types/catalogue/product-category.class";
import {IdDescriptor} from "../id-descriptor.class";
import {IdListDescriptor} from "../id-list-descriptor.class";
import {ListTypeDef} from "../list-type-def.class";
import {TagTypeDef} from "./tag-type-def.class";
import {ProductTypeDef} from "./product-type-def.class";
import {FilterPropertyDefinitionTypeDef} from "./filter-property-definition-type-def.class";
import {ProductImage} from "../../types/catalogue/product-image.class";
import {CatalogueImageTypeDef} from "./catalogue-image-type-def.class";

export class ProductImageTypeDef extends TypeDef<ProductImage> {
  ctor: { new(): ProductImage } = ProductImage;
  service: string = "catalogue";
  route = "product-images";

  id = new IdDescriptor("guid");
  index = 1;
  imageId = new IdDescriptor("guid", "image");
  productId = new IdDescriptor("guid", "product");

  image = CatalogueImageTypeDef;
  product = ProductImageTypeDef;
}
