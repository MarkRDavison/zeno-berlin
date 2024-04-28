FROM mcr.microsoft.com/dotnet/sdk:8.0 as BUILD
WORKDIR /app 

ENV CI_BUILD=true

COPY / /app/



RUN rm web/mark.davison.berlin.web.ui/wwwroot/css/*.css -f
RUN rm web/mark.davison.berlin.web.ui/wwwroot/css/*.min.css -f



RUN ls web/mark.davison.berlin.web.ui/wwwroot
RUN ls web/mark.davison.berlin.web.ui/wwwroot/css



RUN dotnet tool install Excubo.WebCompiler --global
RUN /root/.dotnet/tools/webcompiler web/mark.davison.berlin.web.ui/wwwroot/css/app.scss -m -o web/mark.davison.berlin.web.ui/wwwroot/css -z disable



RUN ls web/mark.davison.berlin.web.ui/wwwroot
RUN ls web/mark.davison.berlin.web.ui/wwwroot/css



RUN dotnet restore web/mark.davison.berlin.web.ui/mark.davison.berlin.web.ui.csproj
RUN dotnet publish -c Release -o /app/publish/ web/mark.davison.berlin.web.ui/mark.davison.berlin.web.ui.csproj

FROM nginx:alpine AS FINAL
WORKDIR /usr/share/nginx/html
COPY --from=BUILD /app/publish/wwwroot .
COPY web/entry.sh /usr/share/nginx/html/entry.sh
COPY web/nginx.conf /etc/nginx/nginx.conf
RUN ls -l
RUN ls -l ./css

RUN chmod +x /usr/share/nginx/html/entry.sh

WORKDIR /usr/share/nginx/html

CMD ["sh", "entry.sh"]