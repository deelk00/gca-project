import { environment } from 'src/environments/environment';
import { Brand } from './brand.class';
import {ProductImage} from "./product-image.class";

export class CatalogueImage {
  id: string;
  hash: string;
  get source(): string {return new URL("./images/download/" + this.databaseImageId, environment.urls.catalogue).href};
  databaseImageId: string;
  brandIds: string[];
  productImageIds: string[];

  brands?: Brand[];
  productImages?: ProductImage[];
}
