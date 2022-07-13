import { environment } from 'src/environments/environment';
import { Brand } from './brand.class';

export class CatalogueImage {
  id: string;
  hash: string;
  get source(): string {return new URL("./images/download/" + this.databaseImageId, environment.urls.catalogue).href};
  databaseImageId: string;
  brandIds: string[];

  get brands(): Brand[] {

  }
}
