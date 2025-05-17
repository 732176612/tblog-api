FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5901
COPY . .

ENV TimeZone=Asia/Shanghai
ENV ASPNETCORE_ENVIRONMENT=Production
RUN ln -snf /usr/share/zoneinfo/$TimeZone /etc/localtime && echo $TimeZone > /etc/timezone

CMD dotnet TBlog.Api.dll --urls "http://+:5901"