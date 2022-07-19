import { Component, OnInit } from '@angular/core';
import { ShoppingCartService } from '../../services/shopping-cart-service/shopping-cart.service';

@Component({
  selector: 'app-shopping-cart',
  templateUrl: './shopping-cart.component.html',
  styleUrls: ['./shopping-cart.component.scss']
})
export class ShoppingCartComponent implements OnInit {

  constructor(
    public shoppingCart: ShoppingCartService
    ) { }

  ngOnInit(): void {
  }

}
