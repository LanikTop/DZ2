FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Presentation/Presentation.csproj",   "Presentation/"]
COPY ["Application/Application.csproj",   "Application/"]
COPY ["Domain/Domain.csproj",   "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj",   "Infrastructure/"]

RUN dotnet restore "Presentation/Presentation.csproj"

COPY . .
RUN dotnet publish "Presentation/Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && \
    apt-get install -y --no-install-recommends libgssapi-krb5-2 && \
    rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "Presentation.dll"]