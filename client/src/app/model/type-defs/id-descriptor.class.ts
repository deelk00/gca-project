export class IdDescriptor<T> {
  constructor(
    public type: "string" | "number" | "bigint" | "boolean" | "symbol" | "undefined" | "object" | "function" | "guid",
    public propertyName?: keyof T
    ) { }

  accessProp = (t: T) => {
    return this.propertyName ? t[this.propertyName] : t;
  }
}
