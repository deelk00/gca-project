import {TypeDef} from "./type-def.abstract";

export class ListTypeDef<T> extends TypeDef<T> {
  public service: string;
  public ctor: new () => T | (new () => TypeDef<T>);
  public route: string;

  constructor(service: new () => TypeDef<T>)
  constructor(
    service: string,
    ctor: new () => T,
    route: string
  )
  constructor(
    service: string | (new () => TypeDef<T>),
    ctor?: new () => T,
    route?: string
  ) {
    super();
    if(typeof(service) === "string"
      && ctor && route) {
      this.service = service
      this.ctor = ctor;
      this.route = route;
    }else if(typeof(service) !== "string"){
      const td = new service();
      this.service = td.service;
      this.route = td.route;
      this.ctor = service as unknown as new () => T;
    }else {
      throw "error"
    }
  }
}
