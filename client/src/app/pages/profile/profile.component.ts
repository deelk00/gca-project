import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/model/types/authentication/user.class';
import { AuthService } from '../../services/auth-service/auth.service';
import { CrudService } from '../../services/crud-service/crud.service';
import { UserTypeDef } from '../../model/type-defs/authentication/user-type-def.class';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  constructor(
    public crud: CrudService,
    public auth: AuthService
    ) { }

  ngOnInit(): void {

  }

  userChanged = () => {
    this.crud.put(UserTypeDef, this.auth.user)
  }
}
