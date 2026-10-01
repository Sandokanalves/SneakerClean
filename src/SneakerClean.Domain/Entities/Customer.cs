namespace SneakerClean.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Street { get; private set; } = null!;
        public string Neighborhood { get; private set; } = null!;
        public string City { get; private set; } = null!;
        public string ZipCode { get; private set; } = null!;
        public string Phone { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public bool IsActive { get; private set; }

        public ICollection<Order> Orders { get; private set; } = new List<Order>();

        private Customer() { }

        public Customer(string name, string street, string neighborhood, string city, string zipCode, string phone)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome do cliente é obrigatório.", nameof(name));
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Telefone/WhatsApp é obrigatório.", nameof(phone));

            Id = Guid.NewGuid();
            Name = name.Trim();
            Street = street?.Trim() ?? string.Empty;
            Neighborhood = neighborhood?.Trim() ?? string.Empty;
            City = city?.Trim() ?? string.Empty;
            ZipCode = zipCode?.Trim() ?? string.Empty;
            Phone = phone.Trim();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void Update(string name, string street, string neighborhood, string city, string zipCode, string phone)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome do cliente é obrigatório.", nameof(name));
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Telefone/WhatsApp é obrigatório.", nameof(phone));

            Name = name.Trim();
            Street = street?.Trim() ?? string.Empty;
            Neighborhood = neighborhood?.Trim() ?? string.Empty;
            City = city?.Trim() ?? string.Empty;
            ZipCode = zipCode?.Trim() ?? string.Empty;
            Phone = phone.Trim();
        }

        public void SoftDelete()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
    }
}
