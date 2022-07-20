import { TypeDef } from "../type-def.abstract";
import { Payment } from '../../types/checkout/payment.class';
import { IdDescriptor } from "../id-descriptor.class";

export class PaymentTypeDef extends TypeDef<Payment> {
  service = "checkout";
  ctor = Payment;
  route = "";

  id: IdDescriptor<Payment> = new IdDescriptor<Payment>("guid");
  creditCardNumber: string = "";
  creditCardExpirationDate: string = "";
  pin: string = "";
  paymentDate: string = "";
}
