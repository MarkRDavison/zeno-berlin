FROM mcr.microsoft.com/dotnet/sdk:10.0 as BUILD
WORKDIR /app 

COPY / /app/
RUN dotnet restore ./zeno-berlin.slnx
RUN dotnet publish -c Release -o out api/mark.davison.berlin.api.jobs/mark.davison.berlin.api.jobs.csproj

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled
WORKDIR /app
COPY --from=BUILD /app/out .

ENTRYPOINT ["dotnet", "mark.davison.berlin.api.jobs.dll"]