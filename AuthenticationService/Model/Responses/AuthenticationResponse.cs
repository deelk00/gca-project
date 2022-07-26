using AuthenticationService.Model.Database.Types;

namespace AuthenticationService.Model.Responses;

public class AuthenticationResponse
{
    public string AuthToken { get; set; }
    public string RefreshToken { get; set; }
    public uint AuthExpiresIn { get; set; }
    public uint RefreshExpiresIn { get; set; }
    public User User { get; set; }
}