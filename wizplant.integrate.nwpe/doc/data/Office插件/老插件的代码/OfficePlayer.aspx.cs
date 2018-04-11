using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PR.WizPlant.Web.SystemModule.DataModelCenter
{
    public partial class OfficePlayer : System.Web.UI.Page
    {
        public string url = "";
        public string fileType = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            url = Request["fileUri"];
            fileType = Path.GetExtension(url);
        }
    }
}