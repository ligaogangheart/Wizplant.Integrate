using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    public interface IScadaService<T>
    {
        void Insert(T data);
        Task InsertAsync(T data);
        void InsertList(IList<T> list);
        Task InsertListAsync(IList<T> list);
        T Get(string tagNo);
        Task<T> GetAsync(string tagNo);
        IList<T> GetList(string[] tagNos);
        Task<IList<T>> GetListAsync(string[] tagNos);        
    }
}
