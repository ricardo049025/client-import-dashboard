using Domain.Entities;

namespace Domain.Domain.Interfaces.Services;

public  interface IGenresService
{
    Task<IEnumerable<Genre>> GetAllGenresAsync();
}
