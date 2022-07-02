#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TBlog.Api/TBlog.Api.csproj", "TBlog.Api/"]
COPY ["TBlog.TencentCloud/TBlog.TencentCloud.csproj", "TBlog.TencentCloud/"]
COPY ["TBlog.Common/TBlog.Common.csproj", "TBlog.Common/"]
COPY ["TBlog.Extensions/TBlog.Extensions.csproj", "TBlog.Extensions/"]
COPY ["TBlog.Tasks/TBlog.Tasks.csproj", "TBlog.Tasks/"]
COPY ["TBlog.IService/TBlog.IService.csproj", "TBlog.IService/"]
COPY ["TBlog.Model/TBlog.Model.csproj", "TBlog.Model/"]
COPY ["TBlog.Redis/TBlog.Redis.csproj", "TBlog.Redis/"]
COPY ["TBlog.Repository/TBlog.Repository.csproj", "TBlog.Repository/"]
COPY ["TBlog.IRepository/TBlog.IRepository.csproj", "TBlog.IRepository/"]
COPY ["TBlog.EventBus/TBlog.EventBus.csproj", "TBlog.EventBus/"]
COPY ["TBlog.Service/TBlog.Service.csproj", "TBlog.Service/"]
RUN dotnet restore "TBlog.Api/TBlog.Api.csproj"
COPY . .
WORKDIR "/src/TBlog.Api"
RUN dotnet build "TBlog.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TBlog.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TBlog.Api.dll"]