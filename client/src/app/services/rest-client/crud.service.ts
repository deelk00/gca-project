import { Injectable } from '@angular/core';
import {TypeDef} from "../../model/type-defs/type-def.abstract";
import axios from "axios";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class CrudService {
  static instance: CrudService;

  constructor() {
    CrudService.instance ??= this;
  }

  get = async <TDef extends TypeDef<T>, T>(t: new () => TDef, id: any) => {
    const typeDef = new t();
    const entity = await axios.get(
      (environment.urls as any)[typeDef.service] + typeDef.route + "/" + id
      );
    return entity.data as T;
  }

  getList = async <TDef extends TypeDef<T>, T>(t: new () => TDef, page: number = 0) => {
    const typeDef = new t();
    const entities = await axios.get(
      (environment.urls as any)[typeDef.service] + typeDef.route
    );
    return entities.data as T[];
  }

  post = async <TDef extends TypeDef<T>, T>(t: new () => TDef, entity: TDef) => {
    const typeDef = new t();
    const res = await axios.post((environment.urls as any)[typeDef.service] + typeDef.route, entity);
    return res.data as T;
  }

  put = async <TDef extends TypeDef<T>, T>(t: new () => TDef, entity: TDef) => {
    const typeDef = new t();
    const res = await axios.put((environment.urls as any)[typeDef.service] + typeDef.route, entity);
    return res.data as T;
  }

  delete = async <TDef extends TypeDef<T>, T>(t: new () => TDef, id: any) => {
    const typeDef = new t();
    const res = await axios.put((environment.urls as any)[typeDef.service] + typeDef.route + "/" + id);
    return res.data as TDef;
  }
}
