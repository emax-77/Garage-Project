namespace maui_app;

public static class AppConfig
{
    public static ApiOptions Api { get; set; } = new();
}

public class ApiOptions
{
    public string BaseUrl { get; set; } = "http://192.168.1.10:8080";
    public string TokenPath { get; set; } = "/api/auth/token";
    public string OpenPath { get; set; } = "/api/garage/open";
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = "changeme";
}
