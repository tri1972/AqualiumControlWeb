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
        private DateTimeOffset dateLogPeriodStart;
        
        public DateTimeOffset DateLogPeriodStart
        {
            get { return this.dateLogPeriodStart; }
        }
        private DateTimeOffset dateLogPeriodEnd;
        
        public DateTimeOffset DateLogPeriodEnd
        {
            get { return this.dateLogPeriodEnd; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            this.GridViewIOTTable.DataBind();
        }

        protected void ButtonUpdate_Click(object sender, EventArgs e)
        {
            dateLogPeriodStart = DateTimeOffset.Parse(this.txtDate1.Text);
            dateLogPeriodEnd = DateTimeOffset.Parse(this.txtDate2.Text);
            foreach (var log in AzureStrageTable.GetLogDatas(dateLogPeriodStart, dateLogPeriodEnd))
            {
                this.TextBox1.Text += ";" + log;
            }
        }

        protected void ButtonUpdateGridView_Click(object sender, EventArgs e)
        {
            dateLogPeriodStart = DateTimeOffset.Parse(this.txtDate3.Text);
            dateLogPeriodEnd = DateTimeOffset.Parse(this.txtDate4.Text);

            var dataSetAqua= AzureStrageTable.DeserializeAqualiumDataSet(dateLogPeriodStart, dateLogPeriodEnd);

            this.GridViewIOTTable.DataMember = dataSetAqua.Tables[0].TableName;
            this.GridViewIOTTable.DataSource = dataSetAqua;
            this.GridViewIOTTable.DataBind();


        }
        protected void txtDate1_TextChanged(object sender, EventArgs e)
        {
            //このイベントハンドラはカレンダーにバインドされていると、いつの間にかクリアされる場合あるようです
            //dateLogPeriod1 = DateTimeOffset.Parse( this.txtDate1.Text);
        }

        protected void txtDate2_TextChanged(object sender, EventArgs e)
        {
            //このイベントハンドラはカレンダーにバインドされていると、いつの間にかクリアされる場合あるようです
            //dateLogPeriod2 = DateTimeOffset.Parse(this.txtDate2.Text);
        }

        protected void txtDate3_TextChanged(object sender, EventArgs e)
        {

        }

        protected void txtDate4_TextChanged(object sender, EventArgs e)
        {

        }

    }
}