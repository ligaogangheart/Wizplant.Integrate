using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PR.WizPlant.Integrate.Scada.Entities;

namespace PR.WizPlant.ScadaDataGeneral
{
    /// <summary>
    /// 产生数据工作引擎
    /// </summary>
    public class GeneralDataEngineer
    {
        private Random _random;
        public GeneralDataEngineer()
        {
            _random = new Random();
        }

        public float ExecuteGeneralData(string tagId,float oldValue)
        {
            TagValueRule curTagValueRule=null;
            try
            {
                 curTagValueRule = TagValueRuleManager.TagValueRuleDict[tagId.ToUpper()];
            }
            catch (Exception ex)
            {
 
            }
           
            float newValue = curTagValueRule.MinValue;

            if (curTagValueRule.Type == ScadaDataType.YX)
            {
                newValue = _random.Next(0, 100) % 2;
            }
            else
            {
                newValue = ExecuteGeneralNumeric(oldValue, curTagValueRule);
            }

            return newValue;
        }

        public float ExecuteGeneralNumeric(float oldValue, TagValueRule curTagValueRule)
        {
            if (oldValue == curTagValueRule.MinValue)
            {
                return GeneralGreaterNewValue(oldValue, curTagValueRule);
            }
            else if (oldValue == curTagValueRule.MaxValue)
            {
                return GeneralSmallerNewValue(oldValue, curTagValueRule);
            }
            else
            {
                if (_random.Next(0, 100) % 2 == 1)
                {
                    return GeneralGreaterNewValue(oldValue, curTagValueRule);
                }
                else
                {
                    return GeneralSmallerNewValue(oldValue, curTagValueRule);
                }
            }
        }

        public float GeneralGreaterNewValue(float oldValue,TagValueRule curTagValueRule)
        {
            float newValue = curTagValueRule.MaxValue;

            int multiple = (int)((curTagValueRule.MaxValue - oldValue) / curTagValueRule.MinChangeValue);
            int multipleToUse = _random.Next(0,multiple);
            newValue = curTagValueRule.MinChangeValue * multipleToUse+oldValue;

            return newValue;
        }

        public float GeneralSmallerNewValue( float oldValue, TagValueRule curTagValueRule)
        {
            float newValue = curTagValueRule.MinValue;

            int multiple = (int)((oldValue - curTagValueRule.MinValue) / curTagValueRule.MinChangeValue);
            int multipleToUse = _random.Next(0, multiple);
            newValue = oldValue-curTagValueRule.MinChangeValue * multipleToUse;

            return newValue;
        }
    }
}
