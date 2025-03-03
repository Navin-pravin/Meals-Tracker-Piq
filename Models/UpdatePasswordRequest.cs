namespace AljasAuthApi.Models
{
    public class UpdatePasswordRequest
    {
        public required string Email { get; set; }
        public required string NewPassword { get; set; }
    }
}
