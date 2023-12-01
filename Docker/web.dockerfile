FROM node:alpine as BUILD

WORKDIR /app

COPY . ./

RUN npm install
RUN npm run build

FROM nginx:alpine
WORKDIR /usr/share/nginx/html
COPY --from=BUILD /app/dist .
COPY entry.sh /usr/share/nginx/html/entry.sh
COPY nginx.conf /etc/nginx/nginx.conf

RUN chmod +x /usr/share/nginx/html/entry.sh

WORKDIR /usr/share/nginx/html

CMD ["sh", "entry.sh"]