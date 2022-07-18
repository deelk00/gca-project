using System.Security.Cryptography.X509Certificates;
using DynamicQL.Authentication.Model;

namespace DynamicQL.Authentication.Ldap.Model;

public class LdapAuthenticationOptions : AuthenticationOptions
{
    public LdapProtocol Protocol { get; set; } = LdapProtocol.LdapSecure;
    public int Port { get; set; }
    public string Domain { get; set; }
    public string BaseDn { get; set; }
    public X509Certificate2? Certificate { get; set; }
    public string? CertificateThumbprint => Certificate?.Thumbprint;
}