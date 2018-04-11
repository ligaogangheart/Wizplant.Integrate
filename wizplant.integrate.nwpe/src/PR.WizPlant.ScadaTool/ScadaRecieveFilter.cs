using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaTool
{
    public class ScadaRecieveFilter : FixedHeaderReceiveFilter<PackageInfo<ScadaPackageHeader, Byte[]>>
    {
        const int HeaderLength = 11;
        const int LengthStart = 7;
        const int LengthLen = 4;

        byte[] lengthData = new byte[4];         

        public ScadaRecieveFilter()
            : base(HeaderLength)
        {
        }
        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            bufferStream.Skip(LengthStart);
            bufferStream.Read(lengthData, 0, 4);
            return bufferStream.ReadInt32();
        }

        public override PackageInfo<ScadaPackageHeader, byte[]> ResolvePackage(IBufferStream bufferStream)
        {
            ScadaPackageHeader header = new ScadaPackageHeader();
            bufferStream.Skip(6);
            header.PackageType = (byte)bufferStream.ReadByte();
            header.BodyLength = bufferStream.ReadInt32();

            byte[] data = new byte[header.BodyLength];
            bufferStream.Read(data, 0, header.BodyLength);
            return new PackageInfo<ScadaPackageHeader, byte[]>(header,data );
        }
    }
}
