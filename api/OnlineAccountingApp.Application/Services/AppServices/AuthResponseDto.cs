namespace OnlineAccountingApp.Application.Services.AppServices;

public sealed class AuthResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
}
