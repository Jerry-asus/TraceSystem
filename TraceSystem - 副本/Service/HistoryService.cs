using Arch.EntityFrameworkCore.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TraceSystem.Context;
using TraceSystem.Extension;

namespace TraceSystem.Service
{
    public class HistoryService<T> : IHistoryService<T>
    {
        private readonly HistoryContext context;

        public HistoryService(HistoryContext context)
        {
            this.context = context;
        }

        public async Task<bool> AddAsync(ItemNames model)
        {
          var res=  await context.AddAsync(model);
            if(await context.SaveChangesAsync()>0)
            {
                return true;
            }
            return false;
        }

        public bool DeleteAllAsync(int id)
        {
            var res =  context.Remove(id);
            if (context.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public Task<Respostory> DeleteSingleAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Respostory> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Respostory> GetSingleAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Respostory> UpdateAsync(T model)
        {
            throw new NotImplementedException();
        }
    }
}
