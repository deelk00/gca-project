import { TypedDocumentNode } from "apollo-angular";

export abstract class IQueryDefinition<T> {
  service: string;
  query: TypedDocumentNode<unknown, unknown>;
  resolve: { (data: any): T };
}
