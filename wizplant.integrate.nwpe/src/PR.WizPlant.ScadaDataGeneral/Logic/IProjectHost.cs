using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaDataGeneral
{
    /// <summary>
    /// 项目主机接口
    /// </summary>
    interface IProjectHost
    {
        bool ExecuteInit();
        void ExecuteGeneralData();
    }
}
