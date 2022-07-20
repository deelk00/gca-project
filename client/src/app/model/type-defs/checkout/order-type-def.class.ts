import { Order } from '../../types/checkout/order.class';
import { IdDescriptor } from '../id-descriptor.class';
import { IdListDescriptor } from '../id-list-descriptor.class';
import { TypeDef } from '../type-def.abstract';
import { OrderStatus } from '../../enums/order-status.enum';
import { PaymentTypeDef } from './payment-type-def.class';

export class OrderTypeDef extends TypeDef<Order> {
  service = "checkout";
  ctor = Order;
  route = "orders";

  id: IdDescriptor<Order> = new IdDescriptor<Order>("guid");
  cartId = new IdDescriptor<Order>("guid");
  orderStatus: OrderStatus;
  paymentId = new IdDescriptor<Order>("guid", "payment");
  payment = PaymentTypeDef;
}
