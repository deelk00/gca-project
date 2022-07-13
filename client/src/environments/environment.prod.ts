const environment = {
  production: true,
  host: "http://localhost:8080/",
  urls: {
    catalogue: ""
  }
};

environment.urls.catalogue = new URL("api/catalogue", environment.host).href;

export {
  environment
}
