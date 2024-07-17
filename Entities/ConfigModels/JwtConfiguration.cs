namespace Entities.ConfigModels
{
    public class JwtConfiguration
    {
        public string? SectionName { get; set; } = "JwtSettings";
        public string? ValidIssuer { get; set; }
        public string? ValidAudience { get; set; }
        public string? ExpirationTime { get; set; }
    }
}
