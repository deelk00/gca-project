import { Component, OnInit } from '@angular/core';
import { GraphQLService } from '../../services/graph-ql-service/graph-q-l.service';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../model/types/catalogue/product.class';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit {

  productId: string;
  product: BehaviorSubject<Product | null> = new BehaviorSubject<Product | null>(null);

  constructor(
    private graphQL: GraphQLService,
    private activatedRoute: ActivatedRoute
    ) { }

  ngOnInit(): void {
  }

}
