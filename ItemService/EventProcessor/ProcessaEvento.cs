using AutoMapper;
using ItemService.Data;
using ItemService.Dtos;
using ItemService.Models;
using System.Text.Json;

namespace ItemService.EventProcessor
{
    public class ProcessaEvento : IProcessaEvento
    {
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ProcessaEvento(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }

        public void Processa(string mensagem)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();

            // converte a mensagem em um restauranteDto
            var restauranteReadDto = JsonSerializer.Deserialize<RestauranteReadDto>(mensagem);
            //converte para objeto
            var restaurante = _mapper.Map<Restaurante>(restauranteReadDto);

            // se não existir um restaurante com o id será criado um restauranteno banco
            if (!itemRepository.ExisteRestauranteExterno(restaurante.Id))
            {
                itemRepository.CreateRestaurante(restaurante);
                itemRepository.SaveChanges();
            }

        }
    }
}
