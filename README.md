# Garage Project

Tento projekt obsahuje tri casti:
- ESP32 (jednoduchy HTTP server v LAN)
- ASP.NET Core API na Ubuntu serveri (Docker)
- .NET MAUI mobilna appka

Struktura:
```
garage-project/
├── esp32/firmware
├── server/aspnet-api
├── mobile/maui-app
└── README.md
```

## ESP32
- Endpoint: `POST /open`
- Po requeste zopne rele na cca 500 ms a vrati `OK`.

Konfiguracia:
- Uprav [esp32/firmware/include/config.h](esp32/firmware/include/config.h)

Build a nahratie (PlatformIO):
- Otvor priecinok [esp32/firmware](esp32/firmware) v PlatformIO
- Upload firmware do ESP32

## Server (ASP.NET Core API)
Konfiguracia v [server/aspnet-api/appsettings.json](server/aspnet-api/appsettings.json):
- `Garage:Esp32BaseUrl` a `Garage:Esp32TriggerPath`
- `Jwt:*` (SigningKey, username, password)

Lokalne spustenie:
```
dotnet run --project server/aspnet-api
```

Docker (Ubuntu server):
```
cd server/aspnet-api
docker compose up -d --build
```

API:
- `POST /api/auth/token` (body: `{ "username": "...", "password": "..." }`)
- `POST /api/garage/open` (Bearer token)

## Mobilna appka (MAUI)
Konfiguracia v [mobile/maui-app/appsettings.json](mobile/maui-app/appsettings.json):
- `Api:BaseUrl`, `Api:Username`, `Api:Password`

Spustenie (zariadenie/emulator):
```
dotnet build mobile/maui-app
```
