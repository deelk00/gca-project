import { OrderStatus } from '../../enums/order-status.enum';
import { Payment } from './payment.class';
export class Order {
  id: string;
  cartId: string;
  orderStatus: OrderStatus;
  paymentId: string;
  payment: Payment;
}
