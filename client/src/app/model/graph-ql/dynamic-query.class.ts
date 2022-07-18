import {IQueryDefinition} from "./query-definition.interface";
import {gql, TypedDocumentNode} from "apollo-angular";
import pluralize from "pluralize";
import {TypeDef} from "../type-defs/type-def.abstract";
import {ListTypeDef} from "../type-defs/list-type-def.class";
import {IdDescriptor} from "../type-defs/id-descriptor.class";
import {IdListDescriptor} from "../type-defs/id-list-descriptor.class";

const openTypeDefinitions: {[typeName: string]: string} = {};

export enum GraphQLType {
  Nullable = 0,
  NonNullable = 1 << 0,
  String = 1 << 1,
  Int = 1 << 2,
  Guid = 1 << 3,
  Boolean = 1 << 4
}

export class DynamicQuery<TDef extends TypeDef<T>, T> {
  private includeTypes: { [key: string]: DynamicQuery<any, any> } = {};
  private listTypeDef?: ListTypeDef<any>;

  private typeDef: TDef;
  private type: T;

  constructor(
    typeDefCtor: (new (...args: any) => TDef) | TDef | ListTypeDef<T>,
    public variables: {[name: string]: GraphQLType} = {}
  ) {
    const instance = typeof(typeDefCtor) === "function" ? new typeDefCtor() : typeDefCtor;
    if (instance instanceof ListTypeDef) {
      this.listTypeDef = instance;
      this.typeDef = new this.listTypeDef.ctor();
    }else {
      this.typeDef = instance;
    }
    this.type = new this.typeDef.ctor() as T;
  }

  private getQueryName = () => {
    let queryName = this.typeDef.ctor.name;
    queryName = queryName[0].toLowerCase() + queryName.substring(1);
    if(this.listTypeDef) {
      queryName = pluralize(queryName);
    }
    return queryName;
  }

  private getQueryVariableString = () => {
    let varString = "";
    const varKeys = Object.keys(this.variables);
    for (const key of varKeys) {
      varString += `$${key[0] === "$" ? key.substring(1) : key}:`;
      if((this.variables[key] & GraphQLType.Guid) === GraphQLType.Guid) {
        varString += "Guid";
      }
      else if((this.variables[key] & GraphQLType.Int) === GraphQLType.Int) {
        varString += "Int";
      }
      else if((this.variables[key] & GraphQLType.String) === GraphQLType.String) {
        varString += "String";
      }
      else if((this.variables[key] & GraphQLType.Boolean) === GraphQLType.Boolean) {
        varString += "Boolean";
      }
      varString += (this.variables[key] & GraphQLType.NonNullable) === GraphQLType.NonNullable ? "!" : "";
    }

    return varString;
  }

  private getTypeVariableString = () => {
    let varString = "";
    const varKeys = Object.keys(this.variables);
    if(varKeys.length > 0) {
      varString += "(";
      for (const key of varKeys) {
        varString += `${key[0] === "$" ? key.substring(1) : key}:$${key[0] === "$" ? key.substring(1) : key},`;
      }
      varString = varString.substring(0, varString.length - 1) + ")";
    }
    return varString;
  }

  private static getTypeDefinition = <TDef extends TypeDef<T>, T>(typeDef: TDef): string => {
    let typeDefString = openTypeDefinitions[typeDef.ctor.name];
    if(!typeDefString) {
      typeDefString = "";
      for (const key in typeDef) {
        if(key === "service" || key === "route" || key === "ctor") continue;
        const type = typeof(typeDef[key]);
        if(typeDef[key] instanceof IdListDescriptor)
          continue;
        if(typeDef[key] instanceof IdDescriptor
          || (type !== "function"
            && !(typeDef[key] instanceof ListTypeDef)
            && !(typeDef[key] instanceof TypeDef))) {
          typeDefString += key + ","
        }
      }
      openTypeDefinitions[typeDef.ctor.name] = typeDefString;
    }
    typeDefString = typeDefString.substring(0, typeDefString.length -1);
    return typeDefString;
  }

  private getTypeDefinition = () => {
    let typeDefString = DynamicQuery.getTypeDefinition(this.typeDef) + ",";

    for (const key in this.typeDef) {
      if(key === "ctor" || key === "route" || key === "service") continue;
      const type = typeof(this.typeDef[key]);
      if(Object.keys(this.includeTypes).some(x => x === key)
        && (type === "function"
        || this.typeDef[key] instanceof TypeDef
        || this.typeDef[key] instanceof ListTypeDef
      )) {
        typeDefString += DynamicQuery.getInnerQuery((this.includeTypes as any)[key].dynamicQuery, true, key) + ",";
      }
    }
    typeDefString = typeDefString.substring(0, typeDefString.length -1);
    return typeDefString;
  }

  private static getInnerQuery = (dq: DynamicQuery<any, any>, withVariables: boolean = true, name: string | undefined = undefined) => {
    name ??= dq.getQueryName();

    return name + (withVariables ? dq.getTypeVariableString() : "") + "{" + dq.getTypeDefinition() + "}"
  }

  include = <TInDef extends TypeDef<TIn>, TIn>(
    dq: DynamicQuery<TInDef, TIn>
      | (new(...args: any[]) => DynamicQuery<TInDef, TIn>)
      | keyof TDef,
    args: any[] = []
  ): DynamicQuery<TDef, T> => {
    let key: string | undefined;
    if(typeof(dq) === "string") {
      key = dq;
      const td = this.typeDef[key as keyof TDef];
      if(td instanceof TypeDef
        || typeof(td) === "function"){
        dq = (new DynamicQuery(td as any)) as unknown as DynamicQuery<TInDef, TIn>;
      }
      else throw "property name is wrong";
    }
    if(typeof(dq) === "function") dq = new dq(args);
    if(!key && typeof(dq) !== "string") {
      dq = dq as DynamicQuery<TInDef, TIn>;
      const keys = Object.keys(this.typeDef)
        .filter(x => x !== "service"
          && x !== "ctor"
          && x !== "route"
        );
      for (const k of keys) {
        let value = this.typeDef[k as keyof TDef];
        if(!(value instanceof TypeDef)
          && typeof (value) !== "function") continue;

        if(typeof(value) === "function") value = new (value as any)();

        if((value as unknown as TypeDef<any>).ctor.name === dq.typeDef.ctor.name) {
          key = k;
          break;
        }
      }
    }
    if(!key) throw "property name is wrong";
    (this.includeTypes as any)[key] = {
      dynamicQuery: dq
    };

    return this;
  }

  // function to allow TypeScript to follow types correctly
  getMultiQuery = () => this.getQuery() as unknown as IQueryDefinition<T[]>;

  // compiles the query
  getQuery = () => {
    let query: TypedDocumentNode<unknown, unknown>;

    // set default parameters
    if(this.listTypeDef) {
      this.variables["take"] ??= GraphQLType.Int;
      this.variables["skip"] ??= GraphQLType.Int;
      this.variables["page"] ??= GraphQLType.Int;
      this.variables["pageSize"] ??= GraphQLType.Int;
    }else{
      this.variables["id"] ??= GraphQLType.NonNullable | GraphQLType.Guid;
    }
    // default resolve method
    let resolve = (data: any) => data[queryName];

    // get the query name
    let queryName = this.getQueryName();

    // creates the query string
    let queryString = "query(" + this.getQueryVariableString() + "){" + queryName + this.getTypeVariableString() + "{"
      + this.getTypeDefinition() + "}}";
    query = gql(queryString);

    // create the query definition
    const queryDef: IQueryDefinition<T> = {
      service: this.typeDef.service,
      query: query!,
      resolve: resolve
    }

    // return it
    return queryDef;
  }
}
