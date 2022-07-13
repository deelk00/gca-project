export abstract class TypeDef<T> {
    abstract service: string;
    abstract ctor: new () => T;
    abstract route?: string;
}
