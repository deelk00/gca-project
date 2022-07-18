import { Injectable } from '@angular/core';
import {BehaviorSubject} from "rxjs";
import {Product} from "../../model/types/catalogue/product.class";

@Injectable({
  providedIn: 'root'
})
export class ShoppingCartService {
  $cart: BehaviorSubject<{product: Product, count: number}[]>
    = new BehaviorSubject<{product: Product, count: number}[]>( []);

  constructor() { }

  addToCart = (product: Product, count: number) => {
    const current = this.$cart.getValue();
    const value = current.find(x => x.product.id === product.id);
    if(value) {
      value.count += count;
    }else{
      this.$cart.next([...current, {product: product, count: count}]);
    }
  }
  removeFromCart = (product: Product) => {
    this.$cart.next(this.$cart.getValue().filter(x => x.id !== product.id));

  }
}
