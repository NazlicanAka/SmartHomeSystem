namespace SmartHome.API.Infrastructure.Data
{
    public class UserEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }

        // Mülakat Notu: Gerçek projelerde şifreler asla düz metin (plain text) tutulmaz, Hash'lenir (örn: BCrypt). 
        // Ancak vaka çalışmasını karmaşıklaştırmamak için şimdilik düz tutuyoruz.
        public string Password { get; set; }
        public string Role { get; set; } // "Parent" (Ebeveyn) veya "Child" (Çocuk) olacak
    }
}