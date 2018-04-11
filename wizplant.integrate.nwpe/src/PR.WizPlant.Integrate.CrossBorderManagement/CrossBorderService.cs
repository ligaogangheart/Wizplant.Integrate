using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using PR.WizPlant.Integrate.Sql;
using System.Data;
using System.Data.SqlClient;

namespace PR.WizPlant.Integrate.CrossBorderManagement
{
    public class CrossBorderService
    {
        private static Dictionary<string, List<Region>> _siteDangerRegionDict = null;

        static string connectionString = System.Configuration.ConfigurationManager.AppSettings["strConn"];

        public static List<RegionDto> GetAllRegionDtoBySite(string siteId)
        {
            string selectSql = "select Id,RegionName,SiteId,CrossPoints from PRW_Inte_ObjCrossPoint where status=0 and siteId=@siteId";
            SqlParameter para = new SqlParameter("@siteId", siteId);
            List<RegionDto> regions = new List<RegionDto>();
            using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, selectSql, para))
            {
                while (reader.Read())
                {
                    RegionDto region = new RegionDto() { 
                        RegionId=reader.GetGuid(0).ToString(),
                        RegionName = getStringValue(reader,1),
                        SiteId = getStringValue(reader, 2),
                    };
                    string pointsStr = reader.GetString(3);
                    if (!string.IsNullOrEmpty(pointsStr))
                    {
                        List<BorderPointDto> pointArray = new List<BorderPointDto>();
                        string[] points = pointsStr.Split('|');
                        if (points.Length > 0)
                        {
                            for (var i = 0; i < points.Length; i++)
                            {
                                string pointStr = points[i];
                                if (!string.IsNullOrEmpty(pointStr))
                                {
                                    string[] point_xy = pointStr.Split(',');
                                    int point_x = 0;
                                    int point_y = 0;
                                    if (int.TryParse(point_xy[0], out point_x) && int.TryParse(point_xy[1], out point_y))
                                    {
                                        BorderPointDto point = new BorderPointDto(point_x, point_y);
                                        pointArray.Add(point);
                                    }
                                }
                            }
                            region.BorderPointList = pointArray;
                        }
                        
                    }
                    regions.Add(region);
                }
            }
            return regions;
        }

        static string getStringValue(IDataReader reader, int idx)
        {
            if (reader.IsDBNull(idx))
            {
                return null;
            }
            return reader[idx].ToString();
        }

        private static List<Region> GetAllRegionBySite(string siteId)
        {
            string selectSql = "select SiteId,CrossPoints from PRW_Inte_ObjCrossPoint where status=0 and siteId=@siteId";
            SqlParameter para = new SqlParameter("@siteId", siteId);
            List<Region> regions = new List<Region>();
             using (var reader = SqlHelper.ExecuteDataReader(connectionString, CommandType.Text, selectSql,para))
             {
                 while (reader.Read())
                 {
                     string pointsStr = reader.GetString(1);
                     if (!string.IsNullOrEmpty(pointsStr))
                     {
                         List<Point> pointArray = new List<Point>();
                         string[] points = pointsStr.Split('|');
                         if (points.Length > 0)
                         {
                             bool hasError = false;
                             for (var i = 0; i < points.Length; i++)
                             {
                                 string pointStr = points[i];
                                 if (!string.IsNullOrEmpty(pointStr))
                                 {
                                     string[] point_xy = pointStr.Split(',');
                                     int point_x = 0;
                                     int point_z = 0;

                                     if (int.TryParse(point_xy[0], out point_x) && int.TryParse(point_xy[1], out point_z))
                                     {
                                         Point point = new Point(point_x, point_z);
                                         pointArray.Add(point);
                                     }
                                     else
                                     {
                                         hasError = true;
                                         break;
                                     }
                                 }
                             }

                             if (pointArray.Count > 0 && !hasError)
                             {
                                 GraphicsPath tmpPath = new GraphicsPath();
                                 tmpPath.AddPolygon(pointArray.ToArray());
                                 Region tmpRegion = new Region();
                                 tmpRegion.MakeEmpty();
                                 tmpRegion.Union(tmpPath);
                                 regions.Add(tmpRegion);
                             }
                         }
                     }
                 }
             }
             return regions;
        }

        public static bool CheckIsInRegion(PositionGeog positionGeog, string siteId)
        {
            bool result = false;

            PositionModel _standardPositionModel;

            List<Region> regions = GetAllRegionBySite(siteId);
            PositionModel _positionModel = PositionConvert.ConvertToXYCoordinate(positionGeog,siteId,out _standardPositionModel);
            Point point = new Point((int)_positionModel.XPosition,(int)_positionModel.ZPosition);

            foreach (Region region in regions)
            {
                bool tmp = region.IsVisible(point);
                if (tmp)
                {
                    if (_standardPositionModel.YPosition - 2 < _positionModel.YPosition && _positionModel.YPosition< _standardPositionModel.YPosition + 2)
                    {
                        result = true;
                        break;
                    }
                    
                }
            }

            return result;
        }


        public static void AddDangerousRegion(string siteId,string regionName, string regionParam)
        {
            string insertSql = "insert prw_inte_objCrossPoint (Id,SiteId,RegionName,CrossPoints,Status) values (newId(),@siteId,@regionName,@regionParam,0)";
            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@siteId",siteId));
            paramList.Add(new SqlParameter("@regionName", regionName));
            paramList.Add(new SqlParameter("@regionParam",regionParam));

            int result = SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, insertSql, paramList.ToArray());
        }

        public static void DeleteDangerousRegion(string regionId)
        {
            string delelteSql = "delete from prw_inte_objCrossPoint where id = @regionId";
            SqlParameter param = new SqlParameter("@regionId",regionId);
            int result = SqlHelper.ExecuteNonQuery(connectionString,CommandType.Text,delelteSql,param);
        }
    }
}
