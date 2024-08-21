FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5100
EXPOSE 5100
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["todo-backend.csproj", "./"]
RUN dotnet restore "./todo-backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "todo-backend.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "todo-backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "todo-backend.dll"]