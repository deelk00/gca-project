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
    headers?: { [key: string]: any }
  ): BehaviorSubject<T> {
    const queryDefinition: IQueryDefinition<T> = typeof(queryDefinitionParam) === "function"
      ? new queryDefinitionParam()
      : queryDefinitionParam;
    const cacheKey = queryDefinition.service + queryDefinition.query.loc?.source;
    const cache = this.cache.Load(cacheKey);
    const behavior = new BehaviorSubject<T>(cache);

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
