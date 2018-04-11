using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaTool
{
    public class ScadaPackageHeader
    {
        public readonly byte[] SyncWord = new byte[6]{0xEB,0x90, 0xEB,0x90,0xEB,0x90};
        public byte PackageType { get; set; }
        public int BodyLength { get; set; }
    }
}
