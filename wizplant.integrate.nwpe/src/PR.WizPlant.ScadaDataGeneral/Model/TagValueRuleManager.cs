using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaDataGeneral
{
    /// <summary>
    /// 测点值规则管理
    /// </summary>
    public static class TagValueRuleManager
    {
        //tagno为key
        private static Dictionary<string, TagValueRule> _tagValueRuleDict;
        public static Dictionary<string, TagValueRule> TagValueRuleDict
        {
            get
            {
                return _tagValueRuleDict;
            }
            set
            {
                _tagValueRuleDict = value;
            }
        }


    }
}
