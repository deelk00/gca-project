namespace AuthService.Database.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Privilege> Privileges { get; set; }
        public List<User> Users { get; set; }
    }
}
