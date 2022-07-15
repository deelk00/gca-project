import { Component, OnInit } from '@angular/core';
import {BehaviorSubject} from "rxjs";
import {Product} from "../../model/types/catalogue/product.class";

@Component({
  selector: 'app-product-overview',
  templateUrl: './product-overview.component.html',
  styleUrls: ['./product-overview.component.scss']
})
export class ProductOverviewComponent implements OnInit {

  public $products: BehaviorSubject<Product[]> = new BehaviorSubject<Product[]>([]);

  get products() { return this.$products.asObservable(); }

  constructor() {
  }

  ngOnInit(): void {
  }

}
