import {TypeDef} from "./type-def.abstract";

export class ListTypeDef<T> extends TypeDef<T> {
  constructor(
    public service: string,
    public ctor: new () => T,
    public route: string
  ) {
    super();
  }
}
