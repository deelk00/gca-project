import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError } from 'rxjs';
import { UserTypeDef } from 'src/app/model/type-defs/authentication/user-type-def.class';
import { User } from 'src/app/model/types/authentication/user.class';
import { CrudService } from '../crud-service/crud.service';
import { CacheService } from '../cache-service/cache.service';

export enum AuthStatus {
  IsAuthenticated,
  IsNotAuthenticated
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  $authStatus: BehaviorSubject<AuthStatus> = new BehaviorSubject<AuthStatus>(AuthStatus.IsNotAuthenticated);

  get authStatus() { return this.$authStatus.asObservable(); }
  get currentAuthStatus() { return this.$authStatus.getValue(); }

  user?: User;

  constructor(private crud: CrudService, private cache: CacheService) {
    this.user = cache.Load<User>("auth-service:user") ?? undefined;
    if(this.user) {
      this.$authStatus.next(AuthStatus.IsAuthenticated);
    }
  }

  login = (username: string, password: string) => {
    const user = new User();
    user.username = username;
    user.password = password;
    this.crud.post(UserTypeDef, user, "login")
      .pipe(
        catchError(x => {
          this.$authStatus.next(AuthStatus.IsNotAuthenticated);
          throw "User could not be logged in";
        })
      )
      .subscribe(u => {
        this.user = u;
        this.cache.Save("auth-service:user", u);
        this.$authStatus.next(AuthStatus.IsAuthenticated);
      });
  }

  logout = () => {
    this.user = undefined;
    this.cache.clear("auth-service:user");
    this.$authStatus.next(AuthStatus.IsNotAuthenticated);
  }
}
