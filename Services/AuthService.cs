using AljasAuthApi.Config;
using AljasAuthApi.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace AljasAuthApi.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<User> _users;
        private readonly TokenService _tokenService;
        private readonly EmailService _emailService;

        public AuthService(MongoDbSettings settings, TokenService tokenService, EmailService emailService)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>("Users");
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<bool> AuthenticateUser(LoginRequest request)
        {
            var user = await _users.Find(u => u.Email == request.Email && u.Password == request.Password).FirstOrDefaultAsync();
            if (user == null) return false;

            var otp = new Random().Next(1000, 9999).ToString();
            await _emailService.SendOtpAsync(user.Email, otp);

            var update = Builders<User>.Update
                .Set(u => u.OTP, otp)
                .Set(u => u.OTPGeneratedAt, DateTime.UtcNow);
                
            await _users.UpdateOneAsync(u => u.Id == user.Id, update);

            return true;
        }

        public async Task<string> VerifyOTP(OTPRequest request)
        {
            var user = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
            
            if (user == null || string.IsNullOrEmpty(user.OTP) || user.OTPGeneratedAt == null)
                return "Invalid OTP";

            double minutesElapsed = (DateTime.UtcNow - user.OTPGeneratedAt.Value).TotalMinutes;

            if (minutesElapsed > 2)
                return "OTP Expired";

            if (user.OTP != request.OTP)
                return "Invalid OTP";

            // ✅ Generate JWT token on successful login
            return _tokenService.GenerateJwtToken(user.Email);
        }
    }
}
