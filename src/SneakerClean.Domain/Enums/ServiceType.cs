namespace SneakerClean.Domain.Enums
{
    public enum ServiceType
    {
        HigienizacaoPrime = 1,
        Desamarelamento = 2,
        ReparoColagem = 3
    }

    public static class ServiceTypeExtensions
    {
        public static string ToDisplayName(this ServiceType serviceType)
        {
            return serviceType switch
            {
                ServiceType.HigienizacaoPrime => "Higienização Prime",
                ServiceType.Desamarelamento => "Desamarelamento",
                ServiceType.ReparoColagem => "Serviço de Reparo / Colagem",
                _ => serviceType.ToString()
            };
        }

        public static ServiceType ParseServiceType(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return ServiceType.HigienizacaoPrime;

            var normalized = input.Trim().ToLowerInvariant();
            if (normalized.Contains("higieni") || normalized.Contains("prime"))
                return ServiceType.HigienizacaoPrime;
            if (normalized.Contains("desamarel"))
                return ServiceType.Desamarelamento;
            if (normalized.Contains("reparo") || normalized.Contains("colagem"))
                return ServiceType.ReparoColagem;

            if (System.Enum.TryParse<ServiceType>(input, true, out var parsed))
                return parsed;

            return ServiceType.HigienizacaoPrime;
        }
    }
}
