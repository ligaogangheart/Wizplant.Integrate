using Common.Logging;
using PR.WizPlant.Integrate.Scada.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.Scada
{
    public static class BufferHelper
    {
        static ILog logger = LogManager.GetLogger("BufferHelper");
        /// <summary>
        /// 时标起始时间
        /// </summary>
        public static readonly DateTime BeginTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// 判断buffer中从offset开始长度为length的字节是否与flag相等
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="flag"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool IsEqual(byte[] buffer, byte[] flag, int offset, int length)
        {
            if (buffer == null || flag == null)
            {
                return false;
            }

            if (buffer.Length < offset + length || flag.Length != length)
            {
                return false;
            }
            for (int idx = 0; idx < length; idx++)
            {
                if (buffer[offset + idx] != flag[idx])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取byte数据字符串，从低位到高位显示byte十六进制值 
        /// </summary>
        /// <param name="buffer">要显示数据</param>
        /// <returns></returns>
        public static string GetByteStringWith0x(byte[] buffer)
        {
            int len = buffer.Length;
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.AppendFormat("0x{0:X2}", (int)buffer[0]);
            for (int i = 1; i < len; i++)
            {
                sb.AppendFormat(", 0x{0:X2}", (int)buffer[i]);
            }
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 获取byte数据字符串，从低位到高位显示byte十六进制值 
        /// </summary>
        /// <param name="buffer">要显示数据</param>
        /// <returns></returns>
        public static string GetByteString(byte[] buffer)
        {
            int len = buffer.Length;
            StringBuilder sb = new StringBuilder();
            //sb.Append("{");
            sb.AppendFormat("{0:X2}", (int)buffer[0]);
            for (int i = 1; i < len; i++)
            {
                sb.AppendFormat("{0:X2}", (int)buffer[i]);
            }
            //sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 固定4位缓冲
        /// </summary>
        static byte[] fixed4Buffer = new byte[4];

        /// <summary>
        /// 从缓存中读取一个int值
        /// </summary>
        /// <param name="buffer">缓存</param>
        /// <param name="start">开始取值的位置，长度为4个字节</param>
        /// <returns></returns>
        public static int ReadInt32(byte[] buffer, int start)
        {
            int result;
            lock (fixed4Buffer)
            {
                // 由于scada中存储低高位相反，调序
                fixed4Buffer[0] = buffer[start + 3];
                fixed4Buffer[1] = buffer[start + 2];
                fixed4Buffer[2] = buffer[start + 1];
                fixed4Buffer[3] = buffer[start];
                result = BitConverter.ToInt32(fixed4Buffer, 0);
            }
            return result;
        }

        /// <summary>
        /// 从缓存中读取一个float值
        /// </summary>
        /// <param name="buffer">缓存</param>
        /// <param name="start">开始取值的位置，长度为4个字节</param>
        /// <returns></returns>
        public static float ReadSingle(byte[] buffer, int start)
        {
            float result;
            lock (fixed4Buffer)
            {
                // 由于scada中存储低高位相反，调序
                fixed4Buffer[0] = buffer[start + 3];
                fixed4Buffer[1] = buffer[start + 2];
                fixed4Buffer[2] = buffer[start + 1];
                fixed4Buffer[3] = buffer[start];
                result = BitConverter.ToSingle(fixed4Buffer, 0);
            }
            return result;
        }

        /// <summary>
        /// 从缓存中读取一个short值
        /// </summary>
        /// <param name="buffer">缓存</param>
        /// <param name="start">开始取值的位置，长度为2个字节</param>
        /// <returns></returns>
        public static short ReadInt16(byte[] buffer, int start)
        {
            short result;
            lock (fixed4Buffer)
            {
                // 由于scada中存储低高位相反，调序
                fixed4Buffer[0] = buffer[start + 1];
                fixed4Buffer[1] = buffer[start];                
                result = BitConverter.ToInt16(fixed4Buffer, 0);
            }
            return result;
        }

        /// <summary>
        /// 将buffer中从offset开始长度为length的字节转换为字符串返回
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">起始位置</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string GetString(byte[] buffer, int offset, int length)
        {
            return GetString(Encoding.UTF8,buffer,offset,length);
        }

        /// <summary>
        /// 将buffer中从offset开始长度为length的字节转换为字符串返回
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="offset">起始位置</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string GetString(Encoding encoding, byte[] buffer, int offset, int length)
        {
            int i = 0;
            for (; i < length; i++)
            {
                if (buffer[offset + i] == '\0')
                {
                    break;
                }
            }
            if (i < length)
            {
                length = i;
            }

            string result = encoding.GetString(buffer, offset, length).Trim();
            return result;
        }

        static Encoding fullNameEncoding = Encoding.GetEncoding("gb18030");

        /// <summary>
        /// 解析遥信Data
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ParseYXData(byte[] buffer,ScadaData data)
        {
            if (buffer.Length != Consts.YXDataLength)
            {
                logger.ErrorFormat("ParseYXData bufferLength Error:{0}", buffer.Length);
                return false;
            }
            string fullName = null;
            try
            {
                data.TagNo = ReadInt32(buffer, 0).ToString();
                data.MessureType = ReadInt16(buffer, 4);
                //data.FullName = BufferHelper.GetString(buffer, 6, 64);
                fullName = BufferHelper.GetString(fullNameEncoding,buffer, 6, 64);
                string[] names = fullName.Split('.');
                data.Name = names[1];
                data.SiteName = names[0];
                data.Value = buffer[70];
                int seconds = ReadInt32(buffer, 71);
                short millSeconds = ReadInt16(buffer, 75);
                data.TimeStamp = BeginTime.AddSeconds(seconds).AddMilliseconds(millSeconds);
                data.Type = ScadaDataType.YX;
                return true;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("ParseYXData Error:{0}", ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// 解析遥测Data
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ParseYCData(byte[] buffer, ScadaData data)
        {            
            if (buffer.Length != Consts.YCDataLength)
            {
                logger.ErrorFormat("ParseYCData bufferLength Error:{0}", buffer.Length);
                return false;
            }
            string fullName = null;
            try
            {
                data.TagNo = ReadInt32(buffer, 0).ToString();
                data.MessureType = ReadInt16(buffer, 4);
                //data.FullName = BufferHelper.GetString(buffer, 6, 64);
                fullName = BufferHelper.GetString(fullNameEncoding,buffer, 6, 64);
                string[] names = fullName.Split('.');
                data.Name = names[1];
                data.SiteName = names[0];
                data.Value = ReadSingle(buffer, 70);
                int seconds = ReadInt32(buffer, 74);
                short millSeconds = ReadInt16(buffer, 78);
                data.TimeStamp = BeginTime.AddSeconds(seconds).AddMilliseconds(millSeconds);
                data.Type = ScadaDataType.YC;
                return true;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("ParseYCData Error:{0}", ex.Message, ex);
                return false;
            }
        }
  
        /// <summary>
        /// 字节数组Copy
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void FullCopy(byte[] source, byte[] target)
        {
            for (int i = 0; i < source.Length; i++)
            {
                target[i] = source[i];
            }
        }

    }
}
