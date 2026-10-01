using SneakerClean.Domain.Enums;

namespace SneakerClean.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public UserRole Role { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsActive { get; private set; }

        private User() { }

        public User(string name, string email, string passwordHash, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome é obrigatório.", nameof(name));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("E-mail é obrigatório.", nameof(email));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Senha hash é obrigatória.", nameof(passwordHash));

            Id = Guid.NewGuid();
            Name = name.Trim();
            Email = email.Trim().ToLowerInvariant();
            PasswordHash = passwordHash;
            Role = role;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void Update(string name, string email, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome é obrigatório.", nameof(name));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("E-mail é obrigatório.", nameof(email));

            Name = name.Trim();
            Email = email.Trim().ToLowerInvariant();
            Role = role;
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Nova senha é obrigatória.", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
