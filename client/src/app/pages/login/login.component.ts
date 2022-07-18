import { Component, OnInit } from '@angular/core';
import { CrudService } from '../../services/crud-service/crud.service';
import { AuthService, AuthStatus } from '../../services/auth-service/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginText: string = "Login";
  loginButtonIsActive: boolean = true;

  username: string;
  password: string;

  constructor(private auth: AuthService) { }

  ngOnInit(): void {
    const sub = this.auth.authStatus.subscribe(status => {
      if(status === AuthStatus.IsAuthenticated) {
        sub.unsubscribe();
      }else{

      }
    })
  }

  login = () => {
    this.loginButtonIsActive = false;
    this.loginText = "logging in...";

    this.auth.login(this.username, this.password);
  }
}
