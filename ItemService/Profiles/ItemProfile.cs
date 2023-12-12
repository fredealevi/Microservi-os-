using AutoMapper;
using ItemService.Dtos;
using ItemService.Models;

namespace ItemService.Profiles
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<RestauranteReadDto, Restaurante>()
                // informa ao mapper que no atributo IdExterno ser� salvo o Id do restaurante j� criado recebido do Rabbit
                .ForMember(atributo => atributo.IdExterno, recebe => recebe.MapFrom(restauranteDto => restauranteDto.Id));
            CreateMap<Restaurante, RestauranteReadDto>();
            CreateMap<ItemCreateDto, Item>();
            CreateMap<Item, ItemCreateDto>();
        }
    }
}