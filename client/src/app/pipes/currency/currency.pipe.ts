import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'currency'
})
export class CurrencyPipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): unknown | string {
    if(typeof(value) === "string") {
      return value.replace(".", ",");
    }else if(typeof(value) === "number") {
      return value.toFixed(2).replace(".", ",");
    }
    return value;
  }

}
