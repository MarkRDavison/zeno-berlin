FROM mcr.microsoft.com/dotnet/sdk:8.0 as BUILD
WORKDIR /app 

COPY / /app/
RUN dotnet restore ./mark.davison.berlin.sln
RUN dotnet publish -c Release -o out api/mark.davison.berlin.api.jobs/mark.davison.berlin.api.jobs.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled
WORKDIR /app
COPY --from=BUILD /app/out .

ENTRYPOINT ["dotnet", "mark.davison.berlin.api.jobs.dll"]