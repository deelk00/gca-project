import { Gender } from '../../enums/gender.enum';
import { Brand } from './brand.class';
import { Currency } from './currency.class';
import { FilterProperty } from './filter-property.class';
import { ProductCategory } from './product-category.class';
import { Tag } from './tag.class';
import {ProductImage} from "./product-image.class";

export class Product {
  id: string;
  productCategoryId: string;
  currencyId: string;
  brandId?: string;
  name: string;
  stock: number;
  gender: Gender;
  price: number;
  offerPrice?: number;
  description: string;

  tagIds: string[];
  filterPropertyIds: string[];
  productImageIds: string[];

  currency?: Currency;
  productCategory?: ProductCategory;
  tags?: Tag[];
  filterProperties?: FilterProperty[];
  brand?: Brand;
  productImages?: ProductImage[];
}
