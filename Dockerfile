FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY todo-backend.csproj ./

RUN dotnet restore ./todo-backend.csproj

COPY . .

ENV ASPNETCORE_URLS=http://+:5100

EXPOSE 5100

# CMD [ "dotnet", "run"]
