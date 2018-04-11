using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaTool
{
    public static class ScadaConsts
    {
        private static readonly byte[] syncWord = null;

        public static byte[] SyncWord
        {
            get { return syncWord; }
        } 

        static ScadaConsts()
        {
            syncWord = new byte[6] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90 };
        }
    }
}
