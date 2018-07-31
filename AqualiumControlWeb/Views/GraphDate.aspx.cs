using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AqualiumControlWeb.Models;

namespace AqualiumControlWeb.Views
{
    public partial class GraphDate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                //IoTデータの取得を行います
                var dateLogPeriodStart = DateTimeOffset.Parse("2018/07/29");
                var dateLogPeriodEnd = DateTimeOffset.Parse("2018/07/30");

                var dataSetAqua = AzureStrageTable.DeserializeAqualiumDataSet(dateLogPeriodStart, dateLogPeriodEnd);
                var dataTableAqua = dataSetAqua.Tables["aqua"];
                var graphData = new List<DateTime>();
                var graphTemperature = new List<double>();
                var graphPressure = new List<double>();
                var graphHumidity = new List<double>();
                var graphExTemperature = new List<double>();
                for (int i = 0; i < dataTableAqua.Rows.Count; i++)
                {
                    graphData.Add(DateTime.Parse((string)dataTableAqua.Rows[i]["TimeStamp"]));
                    graphTemperature.Add(double.Parse((string)dataTableAqua.Rows[i]["Temperature"]));
                    graphHumidity.Add(double.Parse((string)dataTableAqua.Rows[i]["Humidity"]));
                    graphPressure.Add(double.Parse((string)dataTableAqua.Rows[i]["Pressure"]));
                    graphExTemperature.Add(double.Parse((string)dataTableAqua.Rows[i]["ExternalTemperature"]));
                }

                Chart1.Titles.Add("テスト");

                // Add points to the stock chart series
                for (int index = 0; index < graphData.Count; index++)
                {
                    Chart1.Series["Series1"].Points.AddXY(
                        graphData[index],                // X value is a date
                        graphTemperature[index]    // High Y value
                        );
                    
                    Chart1.Series["Series2"].Points.AddXY(
                        graphData[index],                // X value is a date
                        graphExTemperature[index]    // High Y value
                        );

                }
                for (int index = 0; index < graphData.Count; index++)
                {
                    Chart2.Series["Series1"].Points.AddXY(
                        graphData[index],                // X value is a date
                        graphHumidity[index]    // High Y value
                        );
                    Chart2.Series["Series2"].Points.AddXY(
                        graphData[index],                // X value is a date
                        graphPressure[index]    // High Y value
                        );

                }
            }
            catch(Exception err)
            {
                throw err;
            }
            /*
            // Create a new random number generator
            Random rnd = new Random();

            // Data points X value is using current date
            DateTime date = DateTime.Now.Date;
             
            Chart1.Titles.Add("テスト");

            // Add points to the stock chart series
            for (int index = 0; index < 10; index++)
            {
                Chart1.Series["Series1"].Points.AddXY(
                    date,                // X value is a date
                    rnd.Next(40, 50)    // High Y value
                    );
                date = date.AddDays(1);
            }
            date = DateTime.Now.Date;
            // Add points to the stock chart series
            for (int index = 0; index < 10; index++)
            {
                Chart1.Series["Series2"].Points.AddXY(
                    date,                // X value is a date
                    rnd.Next(40, 50)    // High Y value
                    );
                date = date.AddDays(1);
            }*/

        }
    }
}