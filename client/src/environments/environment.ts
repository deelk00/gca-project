// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

const environment = {
  production: false,
  host: "http://localhost:8080/api/",
  urls: {
    catalogue: "",
    authentication: "",
    checkout: "",
    cart: "",
  }
};

environment.urls.catalogue = joinUrl(environment.host, "/catalogue");
environment.urls.authentication = joinUrl(environment.host, "authentication");
environment.urls.checkout = joinUrl(environment.host, "checkout");
environment.urls.cart = joinUrl(environment.host, "cart");

export {
  environment
}

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
import { joinUrl } from '../app/utility/helper.functions';
