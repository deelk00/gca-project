namespace AuthService.Database.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }

        public List<Role> Roles { get; set; }
    }
}
