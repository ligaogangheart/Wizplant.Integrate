using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    public interface IScadaService<T>
    {
        void Save(T data);
        void Save(IList<T> list);
        T Get(int deviceId);
        IList<T> GetList(int[] deviceIds);
    }
}
