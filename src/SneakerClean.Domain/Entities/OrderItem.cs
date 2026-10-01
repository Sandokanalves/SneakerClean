namespace SneakerClean.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public string SneakerBrand { get; private set; } = null!;
        public string SneakerModel { get; private set; } = null!;
        public string Color { get; private set; } = null!;
        public int Size { get; private set; }
        public string ServiceDescription { get; private set; } = null!;
        public decimal Price { get; private set; }
        public string? Notes { get; private set; }

        private OrderItem() { }

        public OrderItem(string sneakerBrand, string sneakerModel, string color, int size, string serviceDescription, decimal price, string? notes = null)
        {
            if (string.IsNullOrWhiteSpace(sneakerBrand))
                throw new ArgumentException("Marca do tênis é obrigatória.", nameof(sneakerBrand));
            if (string.IsNullOrWhiteSpace(sneakerModel))
                throw new ArgumentException("Modelo do tênis é obrigatório.", nameof(sneakerModel));
            if (price <= 0)
                throw new ArgumentException("O preço do serviço deve ser maior que zero.", nameof(price));

            Id = Guid.NewGuid();
            SneakerBrand = sneakerBrand.Trim();
            SneakerModel = sneakerModel.Trim();
            Color = color?.Trim() ?? string.Empty;
            Size = size;
            ServiceDescription = serviceDescription?.Trim() ?? string.Empty;
            Price = price;
            Notes = notes?.Trim();
        }
    }
}