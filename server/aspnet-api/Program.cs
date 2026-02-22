using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
{
    throw new InvalidOperationException("Jwt:SigningKey is required.");
}

builder.Services.AddSingleton(jwtOptions);
builder.Services.AddSingleton(builder.Configuration.GetSection("Garage").Get<GarageOptions>() ?? new GarageOptions());

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/token", (LoginRequest request, JwtOptions options) =>
{
    if (!string.Equals(request.Username, options.Username, StringComparison.Ordinal) ||
        !string.Equals(request.Password, options.Password, StringComparison.Ordinal))
    {
        return Results.Unauthorized();
    }

    var expiresAt = DateTime.UtcNow.AddMinutes(options.TokenMinutes);
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, request.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
    };

    var credentials = new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
        SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: options.Issuer,
        audience: options.Audience,
        claims: claims,
        expires: expiresAt,
        signingCredentials: credentials);

    var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new TokenResponse(accessToken, expiresAt));
});

app.MapPost("/api/garage/open", async (HttpClient httpClient, GarageOptions garageOptions, CancellationToken cancellationToken) =>
{
    var baseUri = new Uri(garageOptions.Esp32BaseUrl, UriKind.Absolute);
    var targetUri = new Uri(baseUri, garageOptions.Esp32TriggerPath);

    using var response = await httpClient.PostAsync(targetUri, content: null, cancellationToken);
    var body = await response.Content.ReadAsStringAsync(cancellationToken);

    if (!response.IsSuccessStatusCode)
    {
        return Results.Problem(
            detail: string.IsNullOrWhiteSpace(body) ? response.ReasonPhrase : body,
            statusCode: (int)response.StatusCode);
    }

    return Results.Ok(new GarageResponse(string.IsNullOrWhiteSpace(body) ? "OK" : body.Trim()));
}).RequireAuthorization();

app.Run();

record LoginRequest(string Username, string Password);
record TokenResponse(string AccessToken, DateTime ExpiresAt);
record GarageResponse(string Result);

class JwtOptions
{
    public string Issuer { get; set; } = "GarageProject";
    public string Audience { get; set; } = "GarageMobile";
    public string SigningKey { get; set; } = "";
    public int TokenMinutes { get; set; } = 60;
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = "changeme";
}

class GarageOptions
{
    public string Esp32BaseUrl { get; set; } = "http://192.168.1.50";
    public string Esp32TriggerPath { get; set; } = "/open";
}
