FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BMW.CloudAdoption.Sample.Api/BMW.CloudAdoption.Sample.Api.csproj", "BMW.CloudAdoption.Sample.Api/"]
RUN dotnet restore "BMW.CloudAdoption.Sample.Api/BMW.CloudAdoption.Sample.Api.csproj"
COPY . .
WORKDIR "/src/BMW.CloudAdoption.Sample.Api"
RUN dotnet build "BMW.CloudAdoption.Sample.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BMW.CloudAdoption.Sample.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BMW.CloudAdoption.Sample.Api.dll"]
