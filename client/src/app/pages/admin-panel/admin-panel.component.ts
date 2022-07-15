import {AfterViewInit, Component, ElementRef, Input, OnInit, QueryList, TemplateRef, ViewChildren} from '@angular/core';
import {FormBuilder, FormControl} from "@angular/forms";
import {environment} from "../../../environments/environment";
import pluralize from "pluralize";
import {TypeDef} from "../../model/type-defs/type-def.abstract";
import {ProductTypeDef} from "../../model/type-defs/catalogue/product-type-def.class";
import {CatalogueImageTypeDef} from "../../model/type-defs/catalogue/catalogue-image-type-def.class";
import {ProductCategory} from "../../model/types/catalogue/product-category.class";
import {ProductCategoryTypeDef} from "../../model/type-defs/catalogue/product-category-type-def.class";
import {FileChangeEvent} from "@angular/compiler-cli/src/perform_watch";
import {CatalogueImage} from "../../model/types/catalogue/image.class";
import {GraphQLService} from "../../services/graph-ql-service/graph-q-l.service";
import {DynamicQuery} from "../../model/graph-ql/dynamic-query.class";
import {ListTypeDef} from "../../model/type-defs/list-type-def.class";
import {BrandTypeDef} from "../../model/type-defs/catalogue/brand-type-def.class";

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.scss']
})
export class AdminPanelComponent implements AfterViewInit {
  httpMethod: string = "post";
  types: TypeDef<any>[] = [
    new CatalogueImageTypeDef(),
    new ProductTypeDef(),
    new ProductCategoryTypeDef(),
  ]
  type: string = "images";

  images: CatalogueImage[];

  get url() { return environment.urls.catalogue + this.type }
  constructor(
    private graphQL: GraphQLService
  ) {}

  postImageFunc = async (e: Event) => {
    const form = document.getElementById("imageForm") as HTMLFormElement;
    await fetch(environment.urls.catalogue + 'images', {
      method: 'POST',
      body: new FormData(form)
    });
  }

  ngAfterViewInit(): void {
    this.graphQL.executeQuery(
      new DynamicQuery(CatalogueImageTypeDef)
        .include("brands")
        .include("productImages")
        .getMultiQuery()
    )
  }
}
