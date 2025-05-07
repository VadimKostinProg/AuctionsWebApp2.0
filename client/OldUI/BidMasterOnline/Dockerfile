FROM node:latest as angular
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install -g @angular/cli
RUN npm install
COPY . .
RUN npm run build --configuration=production --force
FROM nginx:latest
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=angular /app/dist/bid-master-online /usr/share/nginx/html