namespace AljasAuthApi.Models
{
    public class UserSummary
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string RoleName { get; set; }
         public List<string> RoleAccess { get; set; }
    }
}
