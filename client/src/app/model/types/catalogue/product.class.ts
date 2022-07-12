import { Gender } from '../../enums/gender.enum';
import { Brand } from './brand.class';
import { Currency } from './currency.class';
import { FilterProperty } from './filter-property.class';
import { ProductCategory } from './product-category.class';
import { Tag } from './tag.class';

export class Product {
  id: string;
  productCategoryId: string;
  currencyId: string;
  brandId?: string;
  name: string;
  stock: number;
  gender: Gender;
  price: number;

  tagIds: string[];
  filterPropertyIds: string[];

  get currency(): Currency {}
  get productCategory(): ProductCategory {}
  get tags(): Tag[] {}
  get filterProperties(): FilterProperty[] {}
  get brand(): Brand {}
}
