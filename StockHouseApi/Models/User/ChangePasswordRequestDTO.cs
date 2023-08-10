namespace StockHouseApi.Models;

public class ChangePasswordRequestDTO
{
    public Guid Id { get; set; }
    public required string Password { get; set; }
}