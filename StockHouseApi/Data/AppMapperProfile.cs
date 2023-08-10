using AutoMapper;
using StockHouseApi.Models;

public class AppMapperProfile : Profile
{
    public AppMapperProfile()
    {
        CreateMap<UserResponseDTO, User>();
        CreateMap<CreateUserRequestDTO, User>();
        CreateMap<GroceryItemDTO, GroceryItem>();
    }
}