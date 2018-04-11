using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using PR.WizPlant.Integrate.Scada.Entities;

namespace PR.WizPlant.ScadaDataGeneral
{
    /// <summary>
    /// 测点值规则模型
    /// </summary>
    public class TagValueRule
    {
        public string Id { get; set; }
        //测量点Id
        public string TagId { get; set; }
        public string TagNo { get; set; }
        public bool IsNumeric { get; set; }
        public float MaxValue { get; set; }
        public float MinValue { get; set; }
        public float MinChangeValue { get; set; }
        public float AbsoluteDifference
        {
            get
            {
                return MaxValue - MinValue;
            }
        }

        public bool IsString { get; set; }
        public string AllString { get; set; }
        public List<string> StringList
        {
            get
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(AllString))
                {
                    list = new List<string>(Regex.Split(AllString, ",;:", RegexOptions.IgnoreCase));
                }
                return list;
            }
        }

        public int MessureType { get; set; }
        public string Name { get; set; }
        public string SiteName { get; set; }
        public ScadaDataType Type { get; set; }
    }
}
