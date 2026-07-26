namespace OnlineAccountingApp.Application.Features.AppFeatures.AuthFeature.Login;

public sealed class AuthResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
}
