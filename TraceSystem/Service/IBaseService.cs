using System.Threading.Tasks;
using TraceSystem.Context;
using TraceSystem.Extension;

namespace TraceSystem.Service
{
    public interface IBaseService<T>
    {
        Task<Respostory> GetAllAsync();

        Task<Respostory> GetSingleAsync(int id);

        Task<Respostory> DeleteAllAsync();

        Task<Respostory> DeleteSingleAsync(int id);

        Task<Respostory> UpdateAsync(T model);

        Task<Respostory> AddAsync(ItemNames model);

    }
}
