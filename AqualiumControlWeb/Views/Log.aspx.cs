using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AqualiumControlWeb.Models;

namespace AqualiumControlWeb.Views
{
    public partial class Log : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonUpdate_Click(object sender, EventArgs e)
        {
            foreach (var log in AzureStrageTable.GetLogDatas())
            {
                this.TextBox1.Text += ";" + log;
            }
        }
    }
}