# Garage Project

Project for opening a garage door using a mobile app (.NET MAUI) connected via a home server (Ubuntu) to a microcontroller (ESP32)

***The project is functional but still evolving – some minor updates are coming soon***

Structure:
```
garage-project/
├── esp32 (firmware - simple HTTP server written in C++)
├── server (ASP.NET Core Web API)
├── mobile (.NET MAUI app)
└── README.md
```
## VS Code extensions (minimal setup):

- Platform IO 
- C#  
- .NET MAUI 

## ESP32
- Endpoint: `POST /open`
- After the request, ESP32 switches the relay for 500 ms and returns `OK`.

Configuration:
- Edit [esp32/firmware/include/config.h](esp32/firmware/include/config.h)

Build and upload:
- Open the [esp32/firmware](esp32/firmware) folder in PlatformIO
- Upload the firmware to the ESP32

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
cd srv/garage-api
docker compose up -d --build
```

API:
- `POST /api/auth/token` (body: `{ "username": "...", "password": "..." }`)
- `POST /api/garage/open` (Bearer token)

## Mobile app (.NET MAUI)
Configuration in [mobile/maui-app/appsettings.json](mobile/maui-app/appsettings.json):
- `Api:BaseUrl`, `Api:Username`, `Api:Password`

Launch (device/emulator):
```
dotnet build mobile/maui-app
```
