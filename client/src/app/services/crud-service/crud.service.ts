import {Injectable} from '@angular/core';
import { TypeDef } from '../../model/type-defs/type-def.abstract';
import axios, {AxiosRequestConfig, AxiosResponse} from "axios";
import {environment} from "../../../environments/environment";
import {BehaviorSubject, from, map, Observable, tap} from "rxjs";
import {CacheService, StorageType} from "../cache-service/cache.service";
import {ListTypeDef} from "../../model/type-defs/list-type-def.class";
import { joinUrl } from '../../utility/helper.functions';

export enum CrudOptions {
  None = 0,
  UseCache = 1,
}

@Injectable({
  providedIn: 'root'
})
export class CrudService {
  static instance: CrudService;

  constructor(private cache: CacheService) {
    CrudService.instance ??= this;
  }

  private request = <T>(
    typeDef: TypeDef<T> | (new () => TypeDef<T>),
    httpMethod: keyof typeof axios,
    getRoute: ((typeDef: TypeDef<T>) => string)
      | string = (typeDef: TypeDef<T>) => joinUrl((environment.urls as any)[typeDef.service], typeDef.route),
    config: AxiosRequestConfig<any> = {},
    body: any = undefined
  ): Observable<AxiosResponse> => {
    if (typeof(typeDef) === "function") typeDef = new typeDef();
    if(body) {
      return from((axios[httpMethod] as any)
        (typeof(getRoute) === "function" ? getRoute(typeDef): getRoute, body, config) ) as Observable<AxiosResponse>;
    }else{
      return from((axios[httpMethod] as any)
        (typeof(getRoute) === "function" ? getRoute(typeDef): getRoute, config)) as Observable<AxiosResponse>;
    }
  }

  private getRequest = <T>(typeDef: TypeDef<T>, route: string, options: CrudOptions) => {
    const request = this.request(typeDef, "get", route);
    const cacheKey = route;
    const behaviorSubject = new BehaviorSubject<T | null>(null);

    if((options & CrudOptions.UseCache) === CrudOptions.UseCache) {
      const cachedValue = this.cache.Load(cacheKey, StorageType.LocalStorage);
      behaviorSubject.next(cachedValue);
    }
    const sub = request.pipe(
      map(x => this.cache.Save(route, x.data, StorageType.LocalStorage)
      )
    ).subscribe(res => {
      behaviorSubject.next(res);
      sub.unsubscribe();
    })

    return behaviorSubject
  }

  get = <T>(t: new () => TypeDef<T>, id: any, options: CrudOptions = CrudOptions.None) => {
    const typeDef = new t();
    const route = joinUrl((environment.urls as any)[typeDef.service], typeDef.route, id);
    return this.getRequest(typeDef, route, options) as BehaviorSubject<T | null>;
  }

  getList = <T>(t: new () => ListTypeDef<T>, page: number = 0, options: CrudOptions = CrudOptions.None) => {
    const typeDef = new t();
    const route = joinUrl((environment.urls as any)[typeDef.service], typeDef.route) + "?page=" + page;
    return this.getRequest(typeDef, route, options) as BehaviorSubject<T[] | null>;
  }

  post = <T>(t: new () =>  TypeDef<T>, entity: T, route?: string): Observable<T> => {
    const typeDef = new t();
    route ??= typeDef.route;
    return from(axios.post(joinUrl((environment.urls as any)[typeDef.service], route), entity))
      .pipe(map(x => x.data as T));
  }

  put = async <T>(t: new () => TypeDef<T>, entity: T) => {
    return this.request(new t(), "put", undefined, undefined, entity);
  }

  delete = async <T>(t: new () => TypeDef<T>, id: any) => {
    const routeFunc = (typeDef: TypeDef<T>) => joinUrl((environment.urls as any)[typeDef.service], typeDef.route, id);
    return this.request(new t(), "delete", routeFunc);
  }
}
