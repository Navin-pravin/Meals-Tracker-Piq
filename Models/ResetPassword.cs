namespace AljasAuthApi.Models
{
    public class PasswordUpdate
    {
        public required string Email { get; set; }
        public required string OTP { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}