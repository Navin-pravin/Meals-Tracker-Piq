namespace AljasAuthApi.Models
{
    public class UpdateUserRequest
    {
        public required string Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string ContactNo { get; set; }
        public required string RoleName { get; set; }
        public required string Password { get; set; }
        public required List<string> RoleAccess { get; set; }
    }
}
