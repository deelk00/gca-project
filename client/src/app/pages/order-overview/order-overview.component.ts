import { Component, OnInit } from '@angular/core';
import axios from 'axios';
import { from, map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { joinUrl } from '../../utility/helper.functions';
import { AuthService } from '../../services/auth-service/auth.service';
import { Order } from '../../model/types/checkout/order.class';

@Component({
  selector: 'app-order-overview',
  templateUrl: './order-overview.component.html',
  styleUrls: ['./order-overview.component.scss']
})
export class OrderOverviewComponent implements OnInit {

  orders: Order[] = [];

  constructor(public auth: AuthService) { }

  ngOnInit(): void {
    //from(axios.get(joinUrl(environment.urls.checkout, "orders", "from-user", this.auth.user!.id)))
    from(axios.get(joinUrl("http://localhost:5030", "orders", "from-user", this.auth.user!.id)))
      .pipe(map(x => x.data as Order[]))
      .subscribe(x => {
        console.log(x);
        
        this.orders = x;
      })
  }

}
