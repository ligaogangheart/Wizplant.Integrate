using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace PR.WizPlant.Integrate.CrossBorderManagement
{
    public class PositionConvert
    {
        private static Dictionary<string,PositionGeog> _standardPositionGeogDict=new Dictionary<string,PositionGeog>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, PositionModel> _standardPositionModelDict = new Dictionary<string, PositionModel>(StringComparer.OrdinalIgnoreCase);

        private static PositionGeog GetStandardPositionBySite(string siteId)
        {
            PositionGeog result=null;

            if (_standardPositionGeogDict.ContainsKey(siteId))
            {
                result = _standardPositionGeogDict[siteId];
            }
            else
            {
                string positionStr = ConfigurationManager.AppSettings["StandardPoint_" + siteId];
                if (!string.IsNullOrEmpty(positionStr))
                {
                    string[] positionValues = positionStr.Split(',');
                    result = new PositionGeog() { 
                        Longitude=double.Parse(positionValues[0]),
                        Latitude=double.Parse(positionValues[1]),
                        Altitude=double.Parse(positionValues[2])
                    };
                    PositionModel positionModel = new PositionModel() { 
                        XPosition=double.Parse(positionValues[3]),
                        ZPosition=double.Parse(positionValues[4]),
                        YPosition=double.Parse(positionValues[5])
                    };
                    if (!_standardPositionGeogDict.ContainsKey(siteId))
                    {
                        _standardPositionGeogDict.Add(siteId,result);
                    };
                    if (!_standardPositionModelDict.ContainsKey(siteId))
                    {
                        _standardPositionModelDict.Add(siteId,positionModel);
                    }
                }
            }

            return result;
        }

        public static string GetModelAxleToLatitudeBySiteId(string siteId)
        {
            return ConfigurationManager.AppSettings["ModelAxle_" + siteId];
        }

        public static PositionModel ConvertToXYCoordinate(PositionGeog position, string siteId, out PositionModel standardPositionModel)
        {
            PositionModel result = new PositionModel();

            PositionGeog standardPositonGeog = GetStandardPositionBySite(siteId);

            double distance_x = CalculateUtilities.GetDistanceByPosition(standardPositonGeog, new PositionGeog() { Longitude = position.Longitude, Latitude = standardPositonGeog.Latitude });
            double distance_y = CalculateUtilities.GetDistanceByPosition(standardPositonGeog, new PositionGeog() { Longitude = standardPositonGeog.Longitude, Latitude = position.Latitude });

            if (position.Longitude < standardPositonGeog.Longitude)
            {
                distance_x = -distance_x;
            }
            if (position.Latitude < standardPositonGeog.Latitude)
            {
                distance_y = -distance_y;
            }

            if (GetModelAxleToLatitudeBySiteId(siteId) != "X")
            {
                double tmp = distance_x;
                distance_x = distance_y;
                distance_y = tmp;
            }

            double axleAngel = double.Parse(ConfigurationManager.AppSettings["AxleAngle_" + siteId]);
            distance_x = distance_x * Math.Cos(axleAngel) + distance_y * Math.Sin(axleAngel);
            distance_y = distance_y * Math.Cos(axleAngel) - distance_x * Math.Sin(axleAngel);

            result.XPosition = _standardPositionModelDict[siteId].XPosition + distance_x;
            result.ZPosition = _standardPositionModelDict[siteId].ZPosition + distance_y;
            result.YPosition = _standardPositionModelDict[siteId].YPosition + (position.Altitude - standardPositonGeog.Altitude);

            standardPositionModel = _standardPositionModelDict[siteId];

            return result;
        }

    }
}
