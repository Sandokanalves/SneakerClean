namespace SneakerClean.Domain.Orders
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid SneakerBrand { get; private set; }
        public Guid SneakerModel { get; private set; }
        public int Size { get; private set; }
        public int ServiceDescription { get; private set; }
        public decimal Price { get; private set; }


        public OrderItem() { }

        public OrderItem(Guid sneakerBrand, Guid sneakerModel, int size, int serviceDescription, decimal price)
        {

            if (price <= 0)
                throw new ArgumentException("O preço do serviço deve ser maior que zero.", nameof(price));

            Id = Guid.NewGuid();
            SneakerBrand = sneakerBrand;
            SneakerModel = sneakerModel;
            Size = size;
            ServiceDescription = serviceDescription;
            Price = price;
        }
    }
}