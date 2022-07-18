using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;

namespace AuthenticationService.Model.Database.Types
{
    [DynamicQL(options: QueryOptions.SingleCrud | QueryOptions.MultiQuery)]
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
