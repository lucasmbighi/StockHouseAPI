namespace StockHouseApi.Models;

public class CreateUserRequestDTO
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}