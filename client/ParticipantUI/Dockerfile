FROM node:latest as angular
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install --force
COPY . .
RUN npm run build

FROM nginx:latest
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=angular /app/dist/participant-ui /usr/share/nginx/html
EXPOSE 4200
CMD ["nginx", "-g", "daemon off;"]
