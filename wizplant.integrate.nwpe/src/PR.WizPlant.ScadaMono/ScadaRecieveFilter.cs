using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.Common;
using System.IO;

namespace PR.WizPlant.ScadaMono
{
    public class ScadaRecieveFilter: FixedHeaderReceiveFilter<BinaryRequestInfo>
    {
        const int HeaderLength = 11;
        const int LengthStart = 7;
        const int LengthLen = 4;
        public ScadaRecieveFilter()
            : base(HeaderLength)
        {

        }

        byte[] lengthData = new byte[4];         

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            int idx = offset + LengthStart;
            lengthData[3] = header[idx++];
            lengthData[2] = header[idx++];
            lengthData[1] = header[idx++];
            lengthData[0] = header[idx];

            int bodyLength = BitConverter.ToInt32(lengthData, 0);

            Console.WriteLine("lengthData:{0:X2},{1:X2},{2:X2},{3:X2}", lengthData[0], lengthData[1], lengthData[2], lengthData[3]);
            
            Console.WriteLine("header.length={0},offset={1},length={2},bodyLength={3}", header.Length, offset, length, bodyLength);        
            
            return bodyLength;
        }


        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
           // Console.WriteLine("header.length={0},offset={1},bodyBuffer.length={2},offset={3},length={4}", header.Array.Length,header.Offset,bodyBuffer.Length, offset, length);
            PackageKey key = (PackageKey)header.Array[header.Offset + 6];
            Console.Write("key is {0}", key.ToString());
            try
            {
                //Console.WriteLine("header.length={0},offset={1},bodyBuffer.length={2},offset={3},length={4}", header.Array.Length, header.Offset, bodyBuffer.Length, offset, length);
                Console.WriteLine("header.length={0},offset={1}", header.Array.Length, header.Offset);
                Console.WriteLine("bodyBuffer.length=,offset={0},length={1}",  offset, length);
                return new BinaryRequestInfo(key.ToString(), bodyBuffer.CloneRange(offset, length));
            }
            catch (Exception ex)
            {
                Console.Write(" ERROR:" + ex.Message);
                return new BinaryRequestInfo(key.ToString(), new byte[]{0x56});
            }
        }
    }
}
