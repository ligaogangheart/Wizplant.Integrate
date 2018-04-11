using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PR.WizPlant.Integrate.WcfHost
{
    public partial class VideoMonitor : System.Web.UI.Page
    {
        protected string host ="10.181.58.239";
        protected string port ="8000";
        protected string camera="0";
        protected string userName = "admin";
        protected string password = "12345";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            host = Request["host"].ToString();
            port = Request["port"].ToString();
            camera = Request["camera"].ToString();
            userName = Request["userName"].ToString();
            password = Request["password"].ToString();
        }
    }
}