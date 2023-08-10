using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockHouseApi.Models
{
    public class GroceryItem
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public double Quantity { get; set; }
        public required UnityType Unity { get; set; }
        public string? Description { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}