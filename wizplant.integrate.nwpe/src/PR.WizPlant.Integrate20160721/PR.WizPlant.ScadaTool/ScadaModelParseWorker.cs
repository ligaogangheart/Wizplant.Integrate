using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace PR.WizPlant.ScadaTool
{
    public class ScadaModelParseWorker : IScadaModelParserWorker
    {
        /// <summary>
        /// 需要集成的厂站名称
        /// </summary>
        string[] inteSites = null;

        private Dictionary<string, string> tagDescs = new Dictionary<string, string>();

        public Dictionary<string, string> TagDescs
        {
            get
            {
                return tagDescs;
            }
        }



        public ScadaModelParseWorker()
        {
              string val = System.Configuration.ConfigurationManager.AppSettings["InteSites"];
            if (!string.IsNullOrEmpty(val))
            {
                inteSites = val.Split(',');
            }
        }

        public void Run()
        {
            string modelFile = System.Configuration.ConfigurationManager.AppSettings["ModelFile"];
            if (string.IsNullOrEmpty(modelFile))
            {
                Console.WriteLine("请先配置ModelFile");
                return;
            }

            if (!File.Exists(modelFile))
            {
                Console.WriteLine("文件：{0} 不存在", modelFile);
                return;
            }
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlSchema schema = new XmlSchema();
            schema.Namespaces.Add("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
            schema.Namespaces.Add("cim", "http://iec.ch/TC57/2003/CIM-schema-cim10#");
            schema.Namespaces.Add("cimEx", "http://dongfang-china.com/UAI#");
            settings.Schemas.Add(schema);
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
           // settings.IgnoreProcessingInstructions = true;
           
            XmlReader reader = XmlReader.Create(modelFile,settings);
            reader.MoveToContent();
            Console.WriteLine("reader.Name={0}", reader.Name);


           
            while (reader.Read())
            {               
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Depth == 1)
                        {
                            parseElement(reader.ReadSubtree());
                        }
                        break;                   
                    default:
                        //Console.WriteLine("Other node {0} with value {1}",
                        //                reader.NodeType, reader.Value);
                        
                        break;
                }
            }

            Console.WriteLine("tagDescs.Count={0}", tagDescs.Count);
            Console.WriteLine("first is {0}={1}", tagDescs.First().Key, tagDescs.First().Value);
        }

        public void parseElement(XmlReader reader)
        {
            string desc = null;
            string name = null;
            string aliasName = null;
            string id = null;
            while (reader.Read())            
            {
                
                if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "cim:Naming.name")
                    {
                        reader.Read();
                        name = reader.Value;
                    }
                    else if (reader.Name == "cim:Naming.description")
                    {
                        reader.Read();
                        desc = reader.Value;
                    }
                    else if (reader.Name == "cim:Naming.aliasName")
                    {
                        reader.Read();
                        aliasName = reader.Value;
                    }
                }else if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element)
                {
                    id = reader.GetAttribute("rdf:ID");
                    if (string.IsNullOrEmpty(id))
                    {
                        return;
                    }
                }
            }
            if (!string.IsNullOrEmpty(id) && name != null && desc != null && aliasName != null)
            {
                if (id.IndexOf(aliasName) == -1)
                {
                    return;
                }

                bool needDeal = true;
                if (inteSites != null)
                {
                    string[] names = name.Split('_');
                    if (names.Length > 2)
                    {
                        if (!inteSites.Contains(string.Format("{0}_{1}", names[0], names[1])))
                        {
                            needDeal = false;
                        }
                    }
                    else
                    {
                        needDeal = false;
                    }
                }
                if (needDeal)
                {
                    tagDescs[aliasName] = desc;
                }
            }

        }

    
    }
}
