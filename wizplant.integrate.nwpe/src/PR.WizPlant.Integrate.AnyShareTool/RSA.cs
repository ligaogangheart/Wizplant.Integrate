using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PR.WizPlant.Integrate.AnyShareTool
{
    class RSA
    {
        /// <summary>
        /// 加密公钥
        /// </summary>
        public static string publickey = @"<RSAKeyValue><Modulus>uyS9A3GjFB7pknYcV08aogAQQgxEYUSSLADwfvs8dSDYEhCjxm3sQ7daI3DQHNHyPhv8k7kHIB9RFvKaLIFJ4tJnExOgp45FW7/CC4ArocvuHrvtpQKQ8EDw/U6+ifJNtUbrtrFleWdVUbkBahpv3Ob2kzkBOVRTiFz1U2mtuZk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        
        /// <summary>
        /// RSA算法加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publickey);

            byte[] cipherbytes;
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);

            // 由于windows 使用的\r\n对 base64进行换行，而服务器rsa只支持 \n换行的base64 串，故将\r\n替换成 \n
            return Convert.ToBase64String(cipherbytes, Base64FormattingOptions.InsertLineBreaks).Replace("\r\n", "\n");
        }
    }
}
