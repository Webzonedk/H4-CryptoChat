namespace API.Models
{
    public class User
    {
        public string? UserName { get; set; } = string.Empty;
        public byte[]? PassWordSalted { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
    }
}
