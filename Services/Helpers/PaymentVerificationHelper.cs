using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;

namespace Services.Helpers
{
    /// <summary>
    /// Helper class for payment verification
    /// </summary>
    public static class PaymentVerificationHelper
    {
        /// <summary>
        /// Validates a credit card number using the Luhn algorithm
        /// </summary>
        /// <param name="cardNumber">The credit card number to validate</param>
        /// <returns>True if the card number is valid according to the Luhn algorithm</returns>
        public static bool ValidateCreditCardNumber(string cardNumber)
        {
            // Remove any non-digit characters
            string digitsOnly = Regex.Replace(cardNumber, @"\D", "");
            
            if (string.IsNullOrEmpty(digitsOnly))
                return false;
                
            // Check for valid length (13-19 digits for most cards)
            if (digitsOnly.Length < 13 || digitsOnly.Length > 19)
                return false;
                
            // Luhn algorithm implementation
            int sum = 0;
            bool alternate = false;
            
            for (int i = digitsOnly.Length - 1; i >= 0; i--)
            {
                int digit = int.Parse(digitsOnly[i].ToString());
                
                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }
                
                sum += digit;
                alternate = !alternate;
            }
            
            return sum % 10 == 0;
        }
        
        /// <summary>
        /// Validates the card expiration date
        /// </summary>
        /// <param name="month">Expiration month (1-12)</param>
        /// <param name="year">Expiration year (4 digits)</param>
        /// <returns>True if the expiration date is valid and not expired</returns>
        public static bool ValidateExpirationDate(int month, int year)
        {
            if (month < 1 || month > 12)
                return false;
                
            if (year < DateTime.Now.Year)
                return false;
                
            if (year == DateTime.Now.Year && month < DateTime.Now.Month)
                return false;
                
            return true;
        }
        
        /// <summary>
        /// Validates the CVV code
        /// </summary>
        /// <param name="cvv">The CVV code to validate</param>
        /// <param name="cardType">The type of card (amex, visa, mastercard, etc.)</param>
        /// <returns>True if the CVV is valid for the given card type</returns>
        public static bool ValidateCvv(string cvv, string cardType)
        {
            if (string.IsNullOrEmpty(cvv) || !Regex.IsMatch(cvv, @"^\d+$"))
                return false;
                
            // American Express uses 4-digit CVVs, others use 3-digit
            if (cardType.ToLower() == "amex")
                return cvv.Length == 4;
                
            return cvv.Length == 3;
        }
        
        /// <summary>
        /// Detects the card type based on the card number
        /// </summary>
        /// <param name="cardNumber">The credit card number</param>
        /// <returns>The detected card type or "Unknown"</returns>
        public static string DetectCardType(string cardNumber)
        {
            string digitsOnly = Regex.Replace(cardNumber, @"\D", "");
            
            if (string.IsNullOrEmpty(digitsOnly))
                return "Unknown";
                
            // Visa
            if (Regex.IsMatch(digitsOnly, @"^4[0-9]{12}(?:[0-9]{3})?$"))
                return "Visa";
                
            // MasterCard
            if (Regex.IsMatch(digitsOnly, @"^5[1-5][0-9]{14}$"))
                return "MasterCard";
                
            // American Express
            if (Regex.IsMatch(digitsOnly, @"^3[47][0-9]{13}$"))
                return "AmericanExpress";
                
            // Discover
            if (Regex.IsMatch(digitsOnly, @"^6(?:011|5[0-9]{2})[0-9]{12}$"))
                return "Discover";
                
            return "Unknown";
        }
        
        /// <summary>
        /// Masks a credit card number for safe display
        /// </summary>
        /// <param name="cardNumber">The credit card number to mask</param>
        /// <returns>The masked card number</returns>
        public static string MaskCardNumber(string cardNumber)
        {
            string digitsOnly = Regex.Replace(cardNumber, @"\D", "");
            
            if (string.IsNullOrEmpty(digitsOnly) || digitsOnly.Length < 4)
                return "****";
                
            // Keep first 6 and last 4 digits, mask the rest
            if (digitsOnly.Length > 10)
            {
                string firstSix = digitsOnly.Substring(0, 6);
                string lastFour = digitsOnly.Substring(digitsOnly.Length - 4, 4);
                int maskedLength = digitsOnly.Length - 10;
                
                return $"{firstSix}{new string('*', maskedLength)}{lastFour}";
            }
            
            // For shorter numbers, just keep last 4 digits
            return $"**{digitsOnly.Substring(digitsOnly.Length - 4, 4)}";
        }
        
        /// <summary>
        /// Logs payment verification for audit purposes
        /// </summary>
        /// <param name="logger">The logger instance</param>
        /// <param name="eventType">The type of payment event</param>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The payment session ID</param>
        /// <param name="isSuccess">Whether the verification was successful</param>
        public static void LogPaymentVerification(ILogger logger, string eventType, string userId, string sessionId, bool isSuccess)
        {
            if (isSuccess)
            {
                logger.LogInformation(
                    "Payment {EventType} successful for user {UserId}, session {SessionId}",
                    eventType, userId, sessionId);
            }
            else
            {
                logger.LogWarning(
                    "Payment {EventType} failed for user {UserId}, session {SessionId}",
                    eventType, userId, sessionId);
            }
        }
    }
} 