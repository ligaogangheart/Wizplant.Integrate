using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Csgii.Entities
{
    public class CsgiiDeviceOfflineDto
    {
        public string ObjId { get; set; }
        public string DeviceId { get; set; }
        public string Station { get; set; }
        public string DeviceName { get; set; }
        public string FullPath { get; set; }
        public string ClassPath { get; set; }
        public int LevelType { get; set; }
        public int Status { get; set; }
    }
}
