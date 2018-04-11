using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR.WizPlant.Integrate.CrossBorderManagement
{
    public class BorderPointDto
    {
        public BorderPointDto(int x, int y)
        {
            XPositon = x;
            YPosition = y;
        }

        public int XPositon { get; set; }
        public int YPosition { get; set; }
    }
}
