import { Component, OnInit } from '@angular/core';
import { GraphQLService } from '../../services/graph-ql-service/graph-q-l.service';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../model/types/catalogue/product.class';
import { BehaviorSubject } from 'rxjs';
import { ProductTypeDef } from '../../model/type-defs/catalogue/product-type-def.class';
import { DynamicQuery, GraphQLType } from '../../model/graph-ql/dynamic-query.class';
import { environment } from 'src/environments/environment';
import { joinUrl } from 'src/app/utility/helper.functions';
import { ShoppingCartComponent } from '../shopping-cart/shopping-cart.component';
import { ShoppingCartService } from '../../services/shopping-cart-service/shopping-cart.service';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit {

  productId: string;
  product?: Product;

  get imageUrl() {return this.product ? joinUrl(environment.urls.catalogue, "images/download/", this.product.productImages![0].imageId) : undefined}

  constructor(
    private graphQL: GraphQLService,
    private activatedRoute: ActivatedRoute,
    public shoppingCart: ShoppingCartService
    ) {
    }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.productId = params["productId"];
      this.graphQL.executeQuery(
        new DynamicQuery<ProductTypeDef, Product>(ProductTypeDef, {"id": GraphQLType.Guid | GraphQLType.NonNullable})
          .include("brand")
          .include("productImages")
          .include("currency")
          .getQuery(),
        {
        id: this.productId
      }).subscribe(x => {
        if(!x) return;
        this.product = x;
        console.log(x);
      })
    });
  }

}
