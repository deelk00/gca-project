import { Injectable } from '@angular/core';
import {IQueryDefinition} from "../../model/graph-ql/query-definition.interface";
import {BehaviorSubject, map, Observable} from "rxjs";
import {Apollo} from "apollo-angular";
import {CacheService} from "../cache-service/cache.service";

@Injectable({
  providedIn: 'root'
})
export class GraphQLService {

  constructor(
    private apollo: Apollo,
    private cache: CacheService
    ) { }

  executeQuery<T>(
    queryDefinitionParam: IQueryDefinition<T> | { new():IQueryDefinition<T> } ,
    variables?: { [key: string]: any },
    headers?: { [key: string]: any },
    useCache: boolean = true
  ): BehaviorSubject<T | null> {
    const queryDefinition: IQueryDefinition<T> = typeof(queryDefinitionParam) === "function"
      ? new queryDefinitionParam()
      : queryDefinitionParam;

    const cacheKey = queryDefinition.service + queryDefinition.query.loc?.source.body + (variables
      ? Object.values(variables).map(x => typeof(x) === "string" || typeof(x) === "number" || typeof(x) === "boolean" ? x
      : undefined).filter(x => x).join(";") : "");

    let behavior = new BehaviorSubject<T | null>(null);
    if(useCache) {
      const cache = this.cache.Load(cacheKey);
      behavior.next(cache);
    }

    const sub = this.apollo.use(queryDefinition.service)
      .query({
        query: queryDefinition.query,
        variables: variables,
        context: {
          headers: headers
        }
      }).pipe(
        map(x => {
          return queryDefinition.resolve(x.data);
        })
      ).subscribe(x => {
        behavior.next(this.cache.Save(cacheKey, x));
        sub.unsubscribe();
      });

    return behavior;
  }
}
