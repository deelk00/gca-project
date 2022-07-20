import { TypeDef } from "../type-def.abstract";
import { User } from '../../types/authentication/user.class';
import { IdDescriptor } from "../id-descriptor.class";
import { IdListDescriptor } from "../id-list-descriptor.class";

export class UserTypeDef extends TypeDef<User> {
  service = "authentication";
  ctor = User;
  route = "users";

  id: IdDescriptor<User> = new IdDescriptor<User>("guid");
  username: string = "";
  password?: string = "";
  zipCode?: string = "";
  street?: string = "";
  number?: string = "";
  country?: string = "";
}
