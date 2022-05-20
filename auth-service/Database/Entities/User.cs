namespace AuthService.Database.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public int UserTag { get; set; }

        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }

        public List<Role> Roles { get; set; }
    }
}
