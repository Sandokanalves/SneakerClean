using SneakerClean.Domain.Address;
using SneakerClean.Domain.Enums;
using SneakerClean.Domain.Orders;

namespace SneakerClean.Domanin.Entities
{
    public class Order
    {
        private readonly List<OrderItem> _items = new();

        public Guid Id { get; private set; }
        public string CustomerName { get; private set; }
        public string CustomerPhone { get; private set; }
        public Address? DeliveryAddress { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        public decimal TotalAmount => _items.Sum(item => item.Price);


        private Order() { }

        public Order(string customerName, string customerPhone, Address? deliveryAddress)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Nome do cliente é obrigatório.", nameof(customerName));

            Id = Guid.NewGuid();
            CustomerName = customerName;
            CustomerPhone = customerPhone;
            DeliveryAddress = deliveryAddress;
            Status = OrderStatus.Received;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddItem(string brand, string model, int size, string serviceDescription, decimal price)
        {
            var item = new OrderItem(brand, model, size, serviceDescription, price);
            _items.Add(item);
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Não é possível alterar o status de uma ordem finalizada ou cancelada.");

            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}