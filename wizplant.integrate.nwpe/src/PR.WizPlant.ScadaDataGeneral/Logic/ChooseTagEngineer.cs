using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.ScadaDataGeneral
{
    /// <summary>
    /// 选择测点引擎
    /// </summary>
    public class ChooseTagEngineer
    {
        private Random _random;
        public ChooseTagEngineer()
        {
            _random = new Random();
        }

        public List<string> ExecuteChooseTagList(IList<string> tagList,int chooseNum)
        {
            List<string> tagChoosedList = new List<string>();

            List<int> random = GeneralRandomIntList(tagList.Count,chooseNum);
            List<int> arrange = ArrangeIntList(random, tagList.Count);

            try
            {
                foreach (int index in arrange)
                {
                    tagChoosedList.Add(tagList[index]);
                }
                //foreach (int index in random)
                //{
                //    tagChoosedList.Add(tagList[index]);
                //}
            }
            catch (Exception ex)
            {
                int i = 3;
            }
           

            return tagChoosedList;
        }

        public List<int> GeneralRandomIntList(int length, int chooseNum)
        {
            List<int> list = new List<int>();
            int tmp;

            for (int i = 0; i < chooseNum; i++)
            {
                tmp = _random.Next(0,length-1);
                list.Add(tmp);
            }

            return list;
        }

        public List<int> ArrangeIntList(IList<int> indexList,int allCount)
        {
            List<int> list = new List<int>();
            Dictionary<int,int> dict=new Dictionary<int,int>();
            int tmp;

            List<int> tmplist = indexList.OrderBy(index => index).ToList();
            foreach (var index in tmplist)
            {
                if (!dict.ContainsKey(index))
                {
                    dict.Add(index, 1);
                    list.Add(index);
                }
                else
                {
                    list.Add(GetIndexRecursive(index,dict,allCount));
                }
            }

            return list;
        }

        public int GetIndexRecursive(int index, Dictionary<int, int> dict,int allCount)
        {
            if (!dict.ContainsKey(index))
            {
                dict.Add(index, 1);
                return index;
            }
            else
            {
                int tmp = index + dict[index];
                dict[index]++;
                if (tmp >= allCount)
                {
                    index = (tmp) % allCount;
                }
                else
                {
                    index = tmp;
                }
                
                return GetIndexRecursive(index,dict,allCount);
            }
        }
    }
}
