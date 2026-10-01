namespace SneakerClean.Domain.Enums
{
    public enum OrderStatus
    {
        Open = 1,        // Aberta
        InProgress = 2,  // Em Andamento
        Completed = 3,   // Concluída
        Cancelled = 4,   // Cancelada
        Delivered = 5    // Entregue
    }
}