namespace Models.DTOs.Authorization
{
    public class UserPhoneRegistrationDTO
    {
        public long PhoneNumber { get; set; }
        public string ConfirmationCode { get; set; }
        public string Password { get; set; }
    }
}
