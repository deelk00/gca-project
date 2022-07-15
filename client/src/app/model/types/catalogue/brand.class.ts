import { CatalogueImage } from "./image.class";
import { Product } from "./product.class";
import {CrudService} from "../../../services/crud-service/crud.service";
import {CatalogueImageTypeDef} from "../../type-defs/catalogue/catalogue-image-type-def.class";

export class Brand {
  id: string;
  imageId: string;
  name: string;
  productIds: string[];

  image?: CatalogueImage;
  products?: Product[];
}
