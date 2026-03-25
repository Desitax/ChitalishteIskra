using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChitalishteIskra.Core.DTOs.Events;

namespace ChitalishteIskra.Core.Contracts
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllAsync();

        Task CreateAsync(CreateEventDto model);

        Task<EventDto?> GetByIdAsync(Guid id);

        Task UpdateAsync(Guid id, CreateEventDto model);

        Task DeleteAsync(Guid id);
    }
}
