export abstract class TypeDef<T> {
    abstract service: string;
    abstract ctor: new () => T | (new () => TypeDef<T>);
    abstract route: string;
}
