namespace AljasAuthApi.Models
{
    public class OTPRequest
    {
        public required string Email { get; set; }
        public required string OTP { get; set; }
    }
}
