using SneakerClean.Domain.ValueObjects;
using SneakerClean.Domain.Enums;

namespace SneakerClean.Domain.Entities
{
    public class Order
    {
        private readonly List<OrderItem> _items = new();

        public Guid Id { get; private set; }
        public string OrderCode { get; private set; } = null!;
        public Guid CustomerId { get; private set; }
        public Customer Customer { get; private set; } = null!;
        public Address? DeliveryAddress { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        public decimal TotalAmount => _items.Sum(item => item.Price);

        private Order() { }

        public Order(Guid customerId, Address? deliveryAddress, string? orderCode = null)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("Cliente é obrigatório para a ordem de serviço.", nameof(customerId));

            Id = Guid.NewGuid();
            CustomerId = customerId;
            DeliveryAddress = deliveryAddress;
            Status = OrderStatus.Open;
            CreatedAt = DateTime.UtcNow;
            OrderCode = orderCode ?? $"OS-{DateTime.UtcNow:yyyyMMdd}-{Id.ToString()[..4].ToUpper()}";
        }

        public void AddItem(string sneakerBrand, string sneakerModel, string color, int size, string serviceDescription, decimal price, string? notes = null)
        {
            var item = new OrderItem(sneakerBrand, sneakerModel, color, size, serviceDescription, price, notes);
            _items.Add(item);
            UpdatedAt = DateTime.UtcNow;
        }

        public void ClearItems()
        {
            _items.Clear();
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            {
                if (newStatus != Status)
                    throw new InvalidOperationException("Não é possível alterar o status de uma ordem finalizada ou cancelada.");
            }

            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}