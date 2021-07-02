FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY Solution.sln ./
COPY Appets.BusinessLogic/*.csproj ./Appets.BusinessLogic/
COPY Appets.BusinessLogic.Interface/*.csproj ./Appets.BusinessLogic.Interface/
COPY Appets.BusinessLogic.Test/*.csproj ./Appets.BusinessLogic.Test/
COPY Appets.DataAccess/*.csproj ./Appets.DataAccess/
COPY Appets.DataAccess.Interface/*.csproj ./Appets.DataAccess.Interface/
COPY Appets.Domain/*.csproj ./Appets.Domain/
COPY Appets.Domain.Interface/*.csproj ./Appets.Domain.Interface/
COPY Appets.Exceptions/*.csproj ./Appets.Exceptions/
COPY Appets.WebApi/*.csproj ./Appets.WebApi/

RUN dotnet restore
COPY . .
WORKDIR /src/Appets.BusinessLogic
RUN dotnet build -c Release -o /app

WORKDIR /src/Appets.BusinessLogic.Interface
RUN dotnet build -c Release -o /app

WORKDIR /src/Appets.BusinessLogic.Test
RUN dotnet build -c Release -o /app

WORKDIR /src/Appets.DataAccess
RUN dotnet build -c Release -o /app

WORKDIR /src/Appets.DataAccess.Interface
RUN dotnet build -c Release -o /app

WORKDIR /src/Appets.Domain
RUN dotnet build -c Release -o /app

WORKDIR /src/Appets.Domain.Interface
RUN dotnet build -c Release -o /app

WORKDIR /src/Appets.Exceptions
RUN dotnet build -c Release -o /app

WORKDIR /src/Appets.WebApi
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Appets.WebApi.dll"]