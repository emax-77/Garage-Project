using System.Net.Http.Json;

namespace maui_app;

public partial class MainPage : ContentPage
{
	private readonly HttpClient _httpClient = new();
	private string? _accessToken;
	private bool _isBusy;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnOpenClicked(object? sender, EventArgs e)
	{
		if (_isBusy)
		{
			return;
		}

		_isBusy = true;
		OpenButton.IsEnabled = false;
		StatusLabel.Text = "Sending...";

		try
		{
			var token = await EnsureTokenAsync();
			if (string.IsNullOrWhiteSpace(token))
			{
				StatusLabel.Text = "Authentication failed.";
				return;
			}

			var request = new HttpRequestMessage(HttpMethod.Post, BuildUrl(AppConfig.Api.OpenPath));
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

			using var response = await _httpClient.SendAsync(request);
			var responseBody = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				StatusLabel.Text = $"Error: {(int)response.StatusCode} {response.ReasonPhrase}";
				return;
			}

			StatusLabel.Text = string.IsNullOrWhiteSpace(responseBody) ? "OK" : responseBody.Trim();
		}
		catch (Exception ex)
		{
			StatusLabel.Text = $"Error: {ex.Message}";
		}
		finally
		{
			OpenButton.IsEnabled = true;
			_isBusy = false;
		}
	}

	private async Task<string?> EnsureTokenAsync()
	{
		if (!string.IsNullOrWhiteSpace(_accessToken))
		{
			return _accessToken;
		}

		var login = new LoginRequest(AppConfig.Api.Username, AppConfig.Api.Password);
		using var response = await _httpClient.PostAsJsonAsync(BuildUrl(AppConfig.Api.TokenPath), login);
		if (!response.IsSuccessStatusCode)
		{
			return null;
		}

		var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
		_accessToken = tokenResponse?.AccessToken;
		return _accessToken;
	}

	private static string BuildUrl(string path)
	{
		var baseUri = new Uri(AppConfig.Api.BaseUrl, UriKind.Absolute);
		return new Uri(baseUri, path).ToString();
	}

	record LoginRequest(string Username, string Password);
	record TokenResponse(string AccessToken, DateTime ExpiresAt);
}
