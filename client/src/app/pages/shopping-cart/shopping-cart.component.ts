import { Component, OnInit } from '@angular/core';
import { ShoppingCartService } from '../../services/shopping-cart-service/shopping-cart.service';
import { GraphQLService } from '../../services/graph-ql-service/graph-q-l.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CrudService } from '../../services/crud-service/crud.service';
import { OrderTypeDef } from '../../model/type-defs/checkout/order-type-def.class';
import axios from 'axios';
import { environment } from 'src/environments/environment';
import { from, map, catchError } from 'rxjs';
import { joinUrl } from '../../utility/helper.functions';
import { Order } from '../../model/types/checkout/order.class';
import { AuthService } from 'src/app/services/auth-service/auth.service';
import { CacheService } from 'src/app/services/cache-service/cache.service';

@Component({
  selector: 'app-shopping-cart',
  templateUrl: './shopping-cart.component.html',
  styleUrls: ['./shopping-cart.component.scss']
})
export class ShoppingCartComponent implements OnInit {
  id?: string;
  processing: boolean = false;

  get price() {return this.shoppingCart.$cart.getValue().length > 0
    ? this.shoppingCart.$cart.getValue().map(x => (x.product.offerPrice ?? x.product.price) * x.count).reduce((acc, cur) => acc + cur)
    : 0
  }

  constructor(
    private cache: CacheService,
    private auth: AuthService,
    private activeRoute: ActivatedRoute,
    public shoppingCart: ShoppingCartService,
    private router: Router,
    private route: ActivatedRoute
    ) { }

  ngOnInit(): void {
    this.route.data.subscribe(d => {
      const id = d["cartId"];

      this.id = id;
      this.shoppingCart = new ShoppingCartService(this.cache, this.auth);
      this.shoppingCart.id = id;
      this.shoppingCart.refreshCart();
    })
  }

  sendOrder = () => {
    if(!this.shoppingCart.id) return;
    this.processing = true;
    from(axios.post(joinUrl("http://localhost:5030/", "orders", this.shoppingCart.id)))
      .pipe(
        map(x => x.data as Order),
        catchError(err => {
          this.processing = false;
          throw err;
        })
      )
      .subscribe(order => {
        this.router.navigate(["/checkout", order.id]);
        this.shoppingCart.clearCart();
      })
  }
}
