import {IdDescriptor} from "./id-descriptor.class";

export class IdListDescriptor<T> extends IdDescriptor<T> {
  constructor(
    type: "string" | "number" | "bigint" | "boolean" | "symbol" | "undefined" | "object" | "function" | "guid",
    propertyName?: keyof T
  ) {
    super(type, propertyName);
  }
}
