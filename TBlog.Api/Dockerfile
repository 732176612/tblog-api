#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5901

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TBlog.Api/TBlog.Api.csproj", "TBlog.Api/"]
COPY ["TBlog.Common/TBlog.Common.csproj", "TBlog.Common/"]
COPY ["TBlog.Extensions/TBlog.Extensions.csproj", "TBlog.Extensions/"]
COPY ["TBlog.Model/TBlog.Model.csproj", "TBlog.Model/"]
COPY ["TBlog.Repository/TBlog.Repository.csproj", "TBlog.Repository/"]
COPY ["TBlog.Service/TBlog.Service.csproj", "TBlog.Service/"]
COPY ["TBlog.RabbitMQ/TBlog.RabbitMQ.csproj", "TBlog.RabbitMQ/"]
RUN dotnet restore "TBlog.Api/TBlog.Api.csproj"
COPY . .
WORKDIR "/src/TBlog.Api"
RUN dotnet build "TBlog.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TBlog.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV TimeZone=Asia/Shanghai
ENV ASPNETCORE_ENVIRONMENT=Production
RUN ln -snf /usr/share/zoneinfo/$TimeZone /etc/localtime && echo $TimeZone > /etc/timezone

CMD dotnet TBlog.Api.dll --urls "http://+:5901"