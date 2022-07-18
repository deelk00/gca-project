import {Component, OnInit} from '@angular/core';
import {CrudService} from "../../services/crud-service/crud.service";
import {ProductCategory} from "../../model/types/catalogue/product-category.class";
import {ProductCategoryTypeDef} from "../../model/type-defs/catalogue/product-category-type-def.class";
import {GraphQLService} from "../../services/graph-ql-service/graph-q-l.service";
import {DynamicQuery, GraphQLType} from "../../model/graph-ql/dynamic-query.class";
import {ListTypeDef} from "../../model/type-defs/list-type-def.class";
import { AuthService, AuthStatus } from '../../services/auth-service/auth.service';
import { Router } from '@angular/router';

export interface ILink {
  id: string;
  name: string;
  route: string;
  subLinks?: ILink[];
}

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss']
})
export class NavigationComponent implements OnInit {
  hoveringLink?: ILink;
  links: ILink[] = []

  constructor(
    private crudService: CrudService,
    private graphQL: GraphQLService,
    public authService: AuthService,
    private router: Router
  ) { }

  setHoveringLink = (e: MouseEvent) => {
    const element = e.currentTarget as HTMLElement
    this.hoveringLink = this.links.find(x => x.id == element.id);
  }
  unsetHoveringLink = (e: MouseEvent) => {
    this.hoveringLink = undefined;
  }

  ngOnInit(): void {
    this.authService.authStatus.subscribe(x => {
      if(x === AuthStatus.IsAuthenticated) {
        this.router.navigate(["/"]);
      }
    });

    const query = new DynamicQuery(new ListTypeDef(ProductCategoryTypeDef), {
      "parentCategoryId": GraphQLType.Guid
    }).getMultiQuery();
    const categoriesObservable = this.graphQL.executeQuery(query, {"parentCategoryId": null});

    const func = (categories: ProductCategory[]) => {
      this.links = [];
      for (const category of categories.filter(x => !x.parentCategoryId)) {
        const link: ILink = {
          id: category.id,
          name: category.name,
          route: "/product-category/" + category.id + "/",
        } as ILink;
        this.links.push(link);
        this.graphQL.executeQuery(query, {"parentCategoryId": category.id})
          .subscribe((x: ProductCategory[] | null) => {
            if(!x) return;
            link.subLinks = x.map(y => {
              return {
                id: y.id,
                name: y.name,
                route: "/product-category/" + y.id,
                subLinks: []
              }
            });
          });
      }
    }
    categoriesObservable.subscribe(x => x ? func([...x]) : null);
    const current = categoriesObservable.getValue();
    if(current) func(current);
  }

}
