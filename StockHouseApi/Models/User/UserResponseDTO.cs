namespace StockHouseApi.Models
{
    public class UserResponseDTO
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
    }
}