import { Injectable } from '@angular/core';
import {BehaviorSubject} from "rxjs";
import { joinUrl } from 'src/app/utility/helper.functions';
import { environment } from 'src/environments/environment';
import { Product } from '../../model/types/catalogue/product.class';
import { CacheService } from '../cache-service/cache.service';

export interface CartProduct {
  product: Product;
  count: number;
}

@Injectable({
  providedIn: 'root'
})
export class ShoppingCartService {
  id?: string;

  $cart: BehaviorSubject<CartProduct[]>
    = new BehaviorSubject<CartProduct[]>( []);

  imageUrls: { [id: string]: string[] } = {};

  get itemCount(): number {
    let count = 0;
    for (const cartProduct of this.$cart.getValue()) {
      count += cartProduct.count;
    }

    return count;
  }

  constructor(private cache: CacheService) {
    const cacheKey = "shopping-cart-service:cached-shopping-cart";
    const cachedValue = cache.Load<CartProduct[]>(cacheKey);
    this.$cart.next(cachedValue ?? []);
    this.$cart.subscribe(value => {
      cache.Save(cacheKey, value);
      this.processProducts(value.map(x => x.product));
    });
  }

  find = (id: string) => {
    return this.$cart.getValue().find(x => x.product.id === id);
  }

  contains = (id: string) => !!this.find(id);

  setCart = (product: Product, count: number = 1) => {
    const p = this.find(product.id);
    if(p) {
      p.count = count;
    }else{
      this.$cart.next([...this.$cart.getValue(), {product: product, count: count}]);
    }
  }

  addToCart = (product: Product, count: number = 1) => {
    const p = this.find(product.id);
    if(p) {
      p.count += count;
    }else{
      this.$cart.next([...this.$cart.getValue(), {product: product, count: count}]);
    }
  }
  removeFromCart = (product: Product) => {
    this.$cart.next(this.$cart.getValue().filter(x => x.product.id !== product.id));

  }

  clearCart = () => {
    this.$cart.next([]);
  }


  processProducts = (products: Product[]) => {
    for (const product of products) {
      if(!product.productImages) continue;
      this.imageUrls[product.id] = [];
      for (const image of product.productImages) {
        this.imageUrls[product.id].push(joinUrl(environment.urls.catalogue, "images/download/", image.imageId));
      }
    }
  }
}
