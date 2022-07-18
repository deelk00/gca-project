import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError } from 'rxjs';
import { UserTypeDef } from 'src/app/model/type-defs/authentication/user-type-def.class';
import { User } from 'src/app/model/types/authentication/user.class';
import { CrudService } from '../crud-service/crud.service';

export enum AuthStatus {
  IsAuthenticated,
  IsNotAuthenticated
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private $authStatus: BehaviorSubject<AuthStatus> = new BehaviorSubject<AuthStatus>(AuthStatus.IsNotAuthenticated);

  get authStatus() { return this.$authStatus.asObservable(); }
  get currentAuthStatus() { return this.$authStatus.getValue(); }

  user?: User;

  constructor(private crud: CrudService) { }

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
        this.$authStatus.next(AuthStatus.IsAuthenticated);
      });
  }

  logout = () => {
    this.user = undefined;
    this.$authStatus.next(AuthStatus.IsNotAuthenticated);
  }
}
