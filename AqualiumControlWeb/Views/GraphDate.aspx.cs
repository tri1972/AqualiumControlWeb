using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AqualiumControlWeb.Views
{
    public partial class GraphDate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Create a new random number generator
            Random rnd = new Random();

            // Data points X value is using current date
            DateTime date = DateTime.Now.Date;

            // Add points to the stock chart series
            for (int index = 0; index < 10; index++)
            {
                Chart1.Series["Series1"].Points.AddXY(
                    date,                // X value is a date
                    rnd.Next(40, 50)    // High Y value
                    );
                date = date.AddDays(1);
            }

        }
    }
}