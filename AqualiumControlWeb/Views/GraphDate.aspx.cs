using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AqualiumControlWeb.Models;

namespace AqualiumControlWeb.Views
{
    public  enum IntervalPeriod
    {
        minutes,
        day,
        week,
        month
    }

    public partial class GraphDate : System.Web.UI.Page
    {
        private DateTime dateTimeNow;
        private DateTimeOffset dateLogPeriodStart;
        private DateTimeOffset dateLogPeriodEnd;

        private List<string> graphData;
        private List<double> graphTemperature;
        private List<double> graphPressure;
        private List<double> graphHumidity;
        private List<double> graphExTemperature;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                this.dateTimeNow = DateTime.Now;
                if (!this.IsPostBack)
                {
                    this.graphData = new List<string>();
                    this.graphTemperature = new List<double>();
                    this.graphPressure = new List<double>();
                    this.graphHumidity = new List<double>();
                    this.graphExTemperature = new List<double>();

                    Label1.Text = "今日の環境データ(" + dateTimeNow.ToString() + " 現在)";
                    this.dateLogPeriodStart = DateTimeOffset.Parse(dateTimeNow.Date.ToString());
                    this.dateLogPeriodEnd = DateTimeOffset.Parse(dateTimeNow.ToString());
                    this.getIoTData(IntervalPeriod.minutes);
                    this.drawGraph();
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        private void getIoTData(IntervalPeriod interval )
        {
            int intervalMinutes;
            string secondUnit="";

            switch (interval)
            {
                case IntervalPeriod.minutes:
                    intervalMinutes = 1;
                    secondUnit = "日";
                    break;
                case IntervalPeriod.day:
                    intervalMinutes = 60;
                    secondUnit = "日";
                    break;
                case IntervalPeriod.week:
                    intervalMinutes = 60 * 24;
                    secondUnit = "月";
                    break;
                default:
                    intervalMinutes = 0;
                    break;
            }

            //IoTデータの取得を行います
            var dataSetAqua = AzureStrageTable.DeserializeAqualiumDataSet(dateLogPeriodStart, dateLogPeriodEnd);
            var dataTableAqua = dataSetAqua.Tables["aqua"];
            this.graphData = new List<string>();
            this.graphTemperature = new List<double>();
            this.graphHumidity = new List<double>();
            this.graphPressure = new List<double>();
            this.graphExTemperature = new List<double>();
            for (int i = 0; i < dataTableAqua.Rows.Count; i++)
            {
                var tmpDateTime = DateTime.Parse((string)dataTableAqua.Rows[i]["TimeStamp"]);
                int totalMinutes = tmpDateTime.Month * 30 + tmpDateTime.Day * 24 + tmpDateTime.Hour * 60 + tmpDateTime.Minute;
                if ((totalMinutes % intervalMinutes) == 0)
                {
                    var tmp = tmpDateTime.Day.ToString() + secondUnit + tmpDateTime.Hour.ToString() + ":" + tmpDateTime.Minute.ToString();
                    graphData.Add(tmp);
                    graphTemperature.Add(double.Parse((string)dataTableAqua.Rows[i]["Temperature"]));
                    graphHumidity.Add(double.Parse((string)dataTableAqua.Rows[i]["Humidity"]));
                    graphPressure.Add(double.Parse((string)dataTableAqua.Rows[i]["Pressure"]));
                    graphExTemperature.Add(double.Parse((string)dataTableAqua.Rows[i]["ExternalTemperature"]));
                }
            }
        }


        private void drawGraph()
        {
            try
            {
                Chart1.Titles.Add("温度");
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
                Chart2.Titles.Add("湿度");
                for (int index = 0; index < graphData.Count; index++)
                {
                    Chart2.Series["Series1"].Points.AddXY(
                        graphData[index],                // X value is a date
                        graphHumidity[index]    // High Y value
                        );

                }
                Chart3.Titles.Add("気圧");
                for (int index = 0; index < graphData.Count; index++)
                {
                    Chart3.Series["Series2"].Points.AddXY(
                        graphData[index],                // X value is a date
                        graphPressure[index]    // High Y value
                        );

                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        /// <summary>
        /// ラジオボタンの選択が変更された場合発生するイベントのハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            IntervalPeriod interval= IntervalPeriod.minutes;
            switch (this.RadioButtonList1.SelectedValue)
            {
                case "0":
                    this.dateLogPeriodStart = DateTimeOffset.Parse(dateTimeNow.Date.ToString());
                    this.dateLogPeriodEnd = DateTimeOffset.Parse(dateTimeNow.ToString());
                    interval = IntervalPeriod.minutes;
                    break;
                case "1":
                    this.dateLogPeriodStart = DateTimeOffset.Parse
                        (dateTimeNow.Date.Subtract(new TimeSpan(7,0,0,0)).ToString());
                    this.dateLogPeriodEnd = DateTimeOffset.Parse(dateTimeNow.ToString());
                    interval = IntervalPeriod.minutes;
                    break;
                case "2":
                    this.dateLogPeriodStart = DateTimeOffset.Parse
                        (dateTimeNow.Date.Subtract(new TimeSpan(30,0,0,0)) .ToString());
                    this.dateLogPeriodEnd = DateTimeOffset.Parse(dateTimeNow.ToString());
                    interval = IntervalPeriod.day;
                    break;
                default:
                    /*
                    this.dateLogPeriodStart = DateTimeOffset.Parse(dateTimeNow.Date.ToString());
                    this.dateLogPeriodEnd = DateTimeOffset.Parse(dateTimeNow.ToString());
                    interval = IntervalPeriod.minutes;
                    */
                    break;
            }
            this.getIoTData(interval);
            this.drawGraph();
        }

        protected void RadioButtonList1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}