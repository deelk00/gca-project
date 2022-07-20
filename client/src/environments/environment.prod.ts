import { joinUrl } from "src/app/utility/helper.functions";

const environment = {
  production: true,
  host: "http://localhost:8080/api",
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
