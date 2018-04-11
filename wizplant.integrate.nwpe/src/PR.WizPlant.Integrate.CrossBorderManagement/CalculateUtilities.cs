using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.CrossBorderManagement
{
    public static class CalculateUtilities
    {
        private const double EARTH_RADIUS=6378.137;

        public static double GetDistanceByPosition(PositionGeog pointA,PositionGeog pointB)
        {
            double radLat1 = Rad(pointA.Latitude);
            double radLat2 = Rad(pointB.Latitude);

            double a = radLat1 - radLat2;
            double b = Rad(pointA.Longitude) - Rad(pointB.Longitude);

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000*1000;

            return s;
        }

        private static double Rad(double d)
        {
            return d * Math.PI / 180.0;
        }
    }
}
