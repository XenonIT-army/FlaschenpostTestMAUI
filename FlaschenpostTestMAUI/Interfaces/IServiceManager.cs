using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaschenpostTestMAUI.Interfaces
{
    public interface IServiceManager<T>
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T dto);
        Task<bool> Update(T dto);
        Task<bool> Delete(T dto);

    }
}
