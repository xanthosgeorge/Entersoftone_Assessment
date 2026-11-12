# Build context: repository root
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY EmployeeManagement.Domain/EmployeeManagement.Domain.csproj EmployeeManagement.Domain/
COPY EmployeeManagement.Application/EmployeeManagement.Application.csproj EmployeeManagement.Application/
COPY EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj EmployeeManagement.Infrastructure/
COPY Entersoftone_Assessment.Api/EmployeeManagement.Api.csproj Entersoftone_Assessment.Api/

RUN dotnet restore Entersoftone_Assessment.Api/EmployeeManagement.Api.csproj

COPY . .
RUN dotnet publish Entersoftone_Assessment.Api/EmployeeManagement.Api.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "EmployeeManagement.Api.dll"]
