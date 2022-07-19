export function joinUrl(...routes: string[]) {
  let host = routes.find(x => x.startsWith("http"));
  if(host) routes.splice(routes.indexOf(host), 1);
  if(host && host[host.length - 1] === "/") host = host.substring(0, host.length - 1);
  let url = host ?? "";

  routes = routes.map(route => {
    if(route[0] === "/") route = route.substring(1);
    if(route[route.length - 1] === "/") route = route.substring(0, route.length - 1);
    return route;
  });

  url += "/" + routes.join("/");

  return url;
}
