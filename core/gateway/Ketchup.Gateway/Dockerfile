FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
ENV ASPNETCORE_URLS http://+:8090
WORKDIR /app
EXPOSE 8090
COPY . .
ENTRYPOINT ["dotnet", "Ketchup.Gateway.dll"]