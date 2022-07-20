import { Component, OnInit } from '@angular/core';
import { ShoppingCartService } from '../../services/shopping-cart-service/shopping-cart.service';
import { GraphQLService } from '../../services/graph-ql-service/graph-q-l.service';
import { Router } from '@angular/router';
import { CrudService } from '../../services/crud-service/crud.service';
import { OrderTypeDef } from '../../model/type-defs/checkout/order-type-def.class';
import axios from 'axios';
import { environment } from 'src/environments/environment';
import { from, map } from 'rxjs';
import { joinUrl } from '../../utility/helper.functions';
import { Order } from '../../model/types/checkout/order.class';

@Component({
  selector: 'app-shopping-cart',
  templateUrl: './shopping-cart.component.html',
  styleUrls: ['./shopping-cart.component.scss']
})
export class ShoppingCartComponent implements OnInit {

  processing: boolean = false;

  get price() {return this.shoppingCart.$cart.getValue().length > 0
    ? this.shoppingCart.$cart.getValue().map(x => (x.product.offerPrice ?? x.product.price) * x.count).reduce((acc, cur) => acc + cur)
    : 0
  }

  constructor(
    public shoppingCart: ShoppingCartService,
    private router: Router
    ) { }

  ngOnInit(): void {

  }

  sendOrder = () => {
    if(!this.shoppingCart.id) return;
    this.processing = true;
    from(axios.post(joinUrl(environment.urls.checkout, this.shoppingCart.id)))
      .pipe(
        map(x => x.data as Order)
      )
      .subscribe(order => {
        this.router.navigate(["/checkout", order.id]);
      })
  }
}
