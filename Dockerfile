FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["WhatShouldIWorkOnToday/WhatShouldIWorkOnToday.csproj", \
      "WhatShouldIWorkOnToday/"]

RUN dotnet restore \
    "WhatShouldIWorkOnToday/WhatShouldIWorkOnToday.csproj"

COPY . .

WORKDIR "/src/WhatShouldIWorkOnToday"

RUN dotnet publish \
    "WhatShouldIWorkOnToday.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false


FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "WhatShouldIWorkOnToday.dll"]