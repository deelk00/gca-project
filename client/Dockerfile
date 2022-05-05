FROM node:lts-alpine3.15 AS build
WORKDIR /app
COPY . .

RUN npm install
RUN npm run build

FROM nginx:latest AS final

COPY --from=build /app/dist/client /usr/share/nginx/html

COPY ./nginx.conf /etc/nginx/nginx.conf