import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import {Product} from "../../model/types/catalogue/product.class";
import { GraphQLService } from '../../services/graph-ql-service/graph-q-l.service';
import { DynamicQuery, GraphQLType } from '../../model/graph-ql/dynamic-query.class';
import { ListTypeDef } from '../../model/type-defs/list-type-def.class';
import { ProductTypeDef } from '../../model/type-defs/catalogue/product-type-def.class';
import { environment } from 'src/environments/environment';
import {ShoppingCartService} from "../../services/shopping-cart-service/shopping-cart.service";
import { joinUrl } from 'src/app/utility/helper.functions';

@Component({
  selector: 'app-product-overview',
  templateUrl: './product-overview.component.html',
  styleUrls: ['./product-overview.component.scss']
})
export class ProductOverviewComponent implements OnInit, AfterViewInit, OnDestroy {
  subs: Subscription[] = [];
  public $products: BehaviorSubject<Product[]> = new BehaviorSubject<Product[]>([]);

  maxPrice: number = 999;
  sortValue: string;

  gender: string = "Mann";

  get genderKeys() {return Object.keys(this.genders)}
  genders: {[name: string]: string} = {
    "Mann": "MALE",
    "Frau": "FEMALe"
  }

  get sortValueKeys() {return Object.keys(this.sortValues)}
  sortValues: {[name: string]: {sortBy: string, ascending: boolean}} = {
    "Preis aufsteigend": {sortBy: "PRICE", ascending: true},
    "Alphabetisch absteigend": {sortBy: "NAME", ascending: !false},
    "Alphabetisch aufsteigend": {sortBy: "NAME", ascending: true}
  }

  imageUrls: { [id: string]: string[] } = {}

  get products() { return this.$products.asObservable(); }

  categoryId: string;

  resizeObserver: ResizeObserver;

  rowSize: number;
  rowsToLoad = 3;
  scrollTolerance = 0;
  currentlyLoading = false;

  constructor(
    private graphQL: GraphQLService,
    private activatedRoute: ActivatedRoute,
    public shoppingCart: ShoppingCartService
  ) {
    this.sortValue = this.sortValueKeys[0];
    window.addEventListener("scroll", this.onScroll);
  }

  onScroll = (e: Event) => {
    if ((window.innerHeight + window.scrollY) >= document.body.scrollHeight - this.scrollTolerance) {
      	this.loadProducts();
    }
  }

  filterChanged = (e: Event) => {
    this.$products.next([]);
    this.loadProducts();
  }

  ngOnDestroy(): void {
    window.removeEventListener("scroll", this.onScroll);
    this.resizeObserver.disconnect();
    this.subs.forEach(x => x.unsubscribe());
  }

  loadProducts = () => {
    if(this.currentlyLoading) return;
    this.currentlyLoading = true;
    const sub = this.graphQL.executeQuery(
      new DynamicQuery<ProductTypeDef, Product>(new ListTypeDef(ProductTypeDef), 
      {
        "productCategoryId": GraphQLType.Guid, 
        "skip": GraphQLType.Int,
        "maxPrice": GraphQLType.Float,
        "gender": "Gender",
        sortBy: "SortBy"
      })
        .include("brand")
        .include("productImages")
        .include("currency")
        .getMultiQuery(),
      {
        "productCategoryId": this.categoryId,
        "skip": this.$products.getValue().length,
        "take": (this.rowSize ?? 1) * this.rowsToLoad,
        "sortBy": this.sortValues[this.sortValue].sortBy,
        "sortByAscending": this.sortValues[this.sortValue].ascending,
        "maxPrice": this.maxPrice,
        "gender": this.genders[this.gender] 
      }, {}, false
    ).subscribe(x => {
      if(!x) return;
      this.currentlyLoading = false;
      this.$products.next([...this.$products.getValue(),...x]);
    });
    this.subs.push(sub);
  }

  ngAfterViewInit(): void {
    let swi = true;
    this.resizeObserver = new ResizeObserver((entries: ResizeObserverEntry[]) => {
      const w = entries.pop()?.contentRect.width!;
      if(w < 400) {
        this.rowSize = 1;
      }else if(w < 700) {
        this.rowSize = 2;
      }else if(w < 1050) {
        this.rowSize = 3;
      }else if(w < 1450) {
        this.rowSize = 4;
      }else{
        this.rowSize = 6;
      }

      if(swi) {
        swi = false;
        this.subs.push(this.$products.subscribe(this.processProducts));
        this.activatedRoute.params.subscribe(params => {
          this.categoryId = params["id"];
          this.$products.next([]);
          this.loadProducts();
        });
      }
    });
    this.resizeObserver.observe(window.document.body);
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

  ngOnInit(): void {

  }


}
