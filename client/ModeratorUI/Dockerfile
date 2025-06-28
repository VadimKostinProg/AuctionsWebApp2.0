FROM node:lts-alpine as angular_builder
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:stable-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=angular_builder /app/dist/moderator-ui /usr/share/nginx/html
EXPOSE 4201
CMD ["nginx", "-g", "daemon off;"]
