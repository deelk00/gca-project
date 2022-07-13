import {Component, OnInit} from '@angular/core';
import { environment } from 'src/environments/environment';
import {CrudService} from "./services/rest-client/crud.service";
import {ProductTypeDef} from "./model/type-defs/catalogue/product-type-def.class";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  constructor(private crudService: CrudService) {
  }

  async ngOnInit(): Promise<void> {
  }
}
