import { Injectable } from '@angular/core';
import { BehaviorSubject, from } from 'rxjs';
import { joinUrl } from 'src/app/utility/helper.functions';
import { environment } from 'src/environments/environment';
import { Product } from '../../model/types/catalogue/product.class';
import { CacheService } from '../cache-service/cache.service';
import { CrudService } from '../crud-service/crud.service';
import { AuthService, AuthStatus } from '../auth-service/auth.service';
import axios from 'axios';

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

  constructor(
    private cache: CacheService,
    private crud: CrudService,
    private auth: AuthService
    ) {
    const cacheKey = "shopping-cart-service:cached-shopping-cart";
    const cachedValue = cache.Load<CartProduct[]>(cacheKey);

    auth.$authStatus.subscribe(x => {
      if(x === AuthStatus.IsAuthenticated && !this.id) {
        from(axios.post(joinUrl(environment.urls.cart, "carts/create", auth.user!.id)))
          .subscribe(cart => {
            this.id = cart.data.id;
          })
      }
    })

    this.$cart.next(cachedValue ?? []);
    this.$cart.subscribe(value => {
      cache.Save(cacheKey, value);
      this.processProducts(value.map(x => x.product));
      console.log("awd");
      if(!this.id) return;
      console.log("awd");

      from(axios.put(joinUrl("http://localhost:5020/", "carts", this.id), this.$cart.getValue().map(x => {return {count: x.count, productId: x.product.id}}))).subscribe(x => {
        console.log(x);

      });
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
    this.$cart.next(this.$cart.getValue().filter(x => x.product.id !== product.id) ?? []);

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
