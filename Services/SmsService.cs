
using Data.Structure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Services
{
    public interface ISmsService
    {
        Task<bool> VerifyConfirmationCode(long phoneNumber, string confirmationCode);
        Task<string> SendSmsAsync(long phoneNumber);
    }

    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;
        private readonly IConfiguration _configuration;
        private readonly MuhamiContext _context;

        public SmsService(ILogger<SmsService> logger, IConfiguration configuration, MuhamiContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public async Task<string> SendSmsAsync(long phoneNumber)
        {
            try
            {
                // Generate a random 6-digit OTP code
                var random = new Random();
                string otpCode = random.Next(100000, 999999).ToString();

                // Create new OTP record
                var otp = new SmsOtpCode
                {
                    PhoneNumber = phoneNumber,
                    Code = otpCode,
                    ExpirationTime = DateTime.UtcNow.AddMinutes(5), // OTP valid for 5 minutes
                    IsUsed = false,
                };

                // Save to database
                _context.SmsOtpCodes.Add(otp);
                await _context.SaveChangesAsync();

                // TODO: Integrate with actual SMS gateway service for Oman
                // For now, just log the OTP
                _logger.LogInformation("OTP code {OtpCode} generated for phone number {PhoneNumber}", otpCode, phoneNumber);

                return otpCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating OTP code for phone number {PhoneNumber}", phoneNumber);
                throw;
            }
        }

        public async Task<bool> VerifyConfirmationCode(long phoneNumber, string confirmationCode)
        {
            try
            {
                if (string.IsNullOrEmpty(confirmationCode))
                {
                    _logger.LogWarning("Confirmation code is null or empty for phone number {PhoneNumber}", phoneNumber);
                    return false;
                }

                var otpCode = await _context.SmsOtpCodes
                    .Where(o => o.PhoneNumber == phoneNumber && !o.IsUsed)
                    .OrderByDescending(o => o.CreateDate)
                    .FirstOrDefaultAsync();

                if (otpCode != null)
                {
                    if (DateTime.UtcNow > otpCode.ExpirationTime)
                    {
                        _logger.LogWarning("OTP code expired for phone number {PhoneNumber}", phoneNumber);
                        return false;
                    }

                    bool isValid = otpCode.Code.Equals(confirmationCode, StringComparison.OrdinalIgnoreCase);
                    if (isValid)
                    {
                        otpCode.IsUsed = true;
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Successfully verified OTP code for phone number {PhoneNumber}", phoneNumber);
                    }
                    else
                    {
                        _logger.LogWarning("Invalid OTP code provided for phone number {PhoneNumber}", phoneNumber);
                    }
                    return isValid;
                }

                _logger.LogWarning("No confirmation code found for phone number {PhoneNumber}", phoneNumber);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying confirmation code for phone number {PhoneNumber}", phoneNumber);
                return false;
            }
        }
        }
}
