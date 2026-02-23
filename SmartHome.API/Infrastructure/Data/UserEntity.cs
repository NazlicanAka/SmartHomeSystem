namespace SmartHome.API.Infrastructure.Data
{
    public class UserEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Parent" (Ebeveyn) veya "Child" (Çocuk) olacak
    }
}