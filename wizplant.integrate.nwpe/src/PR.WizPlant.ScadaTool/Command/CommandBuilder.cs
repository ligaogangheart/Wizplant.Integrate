using SuperSocket.ClientEngine.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaTool.Command
{
    public class CommandBuilder
    {
        static byte[] HELLOdata = new byte[16] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xFF, 0x00, 0x00, 0x00, 0x05, 0x02, 0x9A, 0x03, 0x78, 0x00 };
        static byte[] ALLYXdata = new byte[15] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xC1, 0x00, 0x00, 0x00, 0x04, 0x02, 0x9A, 0x03, 0x78 };
        static byte[] ALLYKdata = new byte[15] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xC3, 0x00, 0x00, 0x00, 0x04, 0x02, 0x9A, 0x03, 0x78 };
        static byte[] MODYXdata = new byte[15] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xC2, 0x00, 0x00, 0x00, 0x04, 0x02, 0x9A, 0x03, 0x78 };
        static byte[] MODYKdata = new byte[15] { 0xEB, 0x90, 0xEB, 0x90, 0xEB, 0x90, 0xC4, 0x00, 0x00, 0x00, 0x04, 0x02, 0x9A, 0x03, 0x78 };


        public static BinaryCommandInfo CreateHELLO()
        {
            BinaryCommandInfo ci = new BinaryCommandInfo("HELLO", HELLOdata);
            return ci;
        }

        public static BinaryCommandInfo CreateALLYX()
        {
            BinaryCommandInfo ci = new BinaryCommandInfo("ALLYX", ALLYXdata);
            return ci;
        }

        public static BinaryCommandInfo CreateALLYK()
        {

            BinaryCommandInfo ci = new BinaryCommandInfo("ALLYK", ALLYKdata);
            return ci;
        }

        public static BinaryCommandInfo CreateMODYX()
        {
            BinaryCommandInfo ci = new BinaryCommandInfo("MODYX", MODYXdata);
            return ci;
        }

        public static BinaryCommandInfo CreateMODYK()
        {
            BinaryCommandInfo ci = new BinaryCommandInfo("MODYK", MODYKdata);
            return ci;
        }
    }
}
