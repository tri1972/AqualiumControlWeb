using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace AqualiumControlWeb.Models
{
    public class DatasetAqualium:DataSet
    {
        private DataTable tableAqua;
        /// <summary>
        /// アクアリウム用テーブルオブジェクト
        /// </summary>
        public DataTable TableAqua
        {
            get { return this.tableAqua; }
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DatasetAqualium(string tableNameAqua) :
            base("default_aquaset")
        {
            this.tableAqua = new DataTable(tableNameAqua);
            this.Tables.Add(tableAqua);
        }

        public void Initialize()
        {

            // データテーブルにカラムを作成・登録
            this.tableAqua.Columns.Add("DeviceId", Type.GetType("System.String"));
            this.tableAqua.Columns.Add("Timestamp", Type.GetType("System.String"));
            this.tableAqua.Columns.Add("Temperature", Type.GetType("System.String"));
            this.tableAqua.Columns.Add("Humidity", Type.GetType("System.String"));
            this.tableAqua.Columns.Add("Pressure", Type.GetType("System.String"));
            this.tableAqua.Columns.Add("ExternalTemperature", Type.GetType("System.String"));

            // データテーブルのプライマリーキー（主キー）を設定
            this.tableAqua.PrimaryKey = new DataColumn[] { this.tableAqua.Columns["col1"] };
        }
    }
}