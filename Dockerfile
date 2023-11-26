FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /hotelNeruda2022
COPY ["proyecto/", "proyecto/"]
COPY ["Datos/", "Datos/"]
COPY ["Entity/", "Entity/"]
COPY ["Logica/", "Logica/"]

RUN dotnet restore "proyecto/proyecto.csproj"
COPY . .
WORKDIR "/hotelNeruda2022/proyecto"
RUN dotnet build -c Release -o /app/build

FROM build AS publish

WORKDIR "/hotelNeruda2022/proyecto"
RUN dotnet publish "proyecto.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "proyecto.dll"]