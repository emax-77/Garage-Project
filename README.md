# Garage Project

Project for opening a garage door using a mobile app (.NET MAUI) connected via a home server (Ubuntu) to a microcontroller (ESP32)

***!!! The project is in the early stage of development - I would like to finish and deploy it in March 2026***

Structure:
```
garage-project/
├── esp32 (firmware - simple HTTP server written in C++)
├── server (ASP.NET Core Web API)
├── mobile (.NET MAUI app)
└── README.md
```

## ESP32
- Endpoint: `POST /open`
- After the request, it closes the relay for 500 ms and returns `OK`.

Configuration:
- Edit [esp32/firmware/include/config.h](esp32/firmware/include/config.h)

Build and upload (PlatformIO):
- Open the [esp32/firmware](esp32/firmware) folder in PlatformIO
- Upload the firmware to ESP32

## Server (ASP.NET Core API)
Configuration in [server/aspnet-api/appsettings.json](server/aspnet-api/appsettings.json):
- `Garage:Esp32BaseUrl` and `Garage:Esp32TriggerPath`
- `Jwt:*` (SigningKey, username, password)

Local launch:
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

## Mobile app (MAUI)
Configuration in [mobile/maui-app/appsettings.json](mobile/maui-app/appsettings.json):
- `Api:BaseUrl`, `Api:Username`, `Api:Password`

Launch (device/emulator):
```
dotnet build mobile/maui-app
```
