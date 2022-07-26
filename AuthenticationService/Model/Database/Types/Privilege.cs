namespace AuthenticationService.Model.Database.Types;

public class Privilege
{
    public Guid Id { get; set; }
    public string AuthKey { get; set; }
    public List<Role> Roles { get; set; }
}