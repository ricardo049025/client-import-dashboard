using Domain.Domain.Interfaces.Repositories;
using Domain.Domain.Interfaces.Services;
using Domain.Entities;

namespace Services.Main;

public class GenresService(IBaseRepository<Genre> genreRepository) : IGenresService
{
    public async Task<IEnumerable<Genre>> GetAllGenresAsync() => await genreRepository.GetAllAsync();

}
