﻿//参考
//https://docs.microsoft.com/ja-jp/azure/storage/storage-dotnet-how-to-use-tables#a-nameprerequisitesa前提条件
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types
using Newtonsoft.Json;

namespace AqualiumControlWeb.Models
{
    public class AzureStrageTable
    {
        private static List<IDictionary<string, EntityProperty>> getTableRecord(DateTimeOffset periodStart, DateTimeOffset periodEnd)
        {
            
            List<IDictionary<string, EntityProperty>> output = new List<IDictionary<string, EntityProperty>>();

            
            
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();



            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("AzureWebJobsHostLogs201707");
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            //TableQuery<aqusaliumRawDate> query = new TableQuery<aqusaliumRawDate>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "I"));

            //ここでテーブルの構造を知らなくてもデータを取ってくることが可能です・・・
            string queryPeriodStart = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, periodStart);
            string queryPeriodEnd = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThanOrEqual, periodEnd);
            string queryPatitionKey = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "I");
            string queryPeriod=TableQuery.CombineFilters(queryPeriodStart, TableOperators.And,queryPeriodEnd);
            string queryWhere = TableQuery.CombineFilters(queryPatitionKey, TableOperators.And, queryPeriod);
            //TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(queryPatitionKey);
            TableQuery <DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(queryWhere);
             // Print the fields for each customer.
            foreach (var entity in table.ExecuteQuery(query))
            {
                entity.Properties.Add("Timestamp",new EntityProperty( entity.Timestamp));//めんどくさいのでpropertiesにTimeStampも追加してしまいます
                output.Add(entity.Properties);

            }

            return output;
        }

        public static List<string> GetLogDatas(DateTimeOffset periodStart, DateTimeOffset periodEnd)
        {
            List<string> output = new List<string>();

            List<IDictionary<string, EntityProperty>> tableRecords = AzureStrageTable.getTableRecord(periodStart, periodEnd);

            foreach (var propertys in tableRecords)
            {
                foreach (var property in propertys)
                {
                    string tmp = "";
                    if (property.Value.PropertyType == EdmType.DateTime)
                    {
                        if (property.Key == "StartTime")
                        {
                            tmp += property.Value.DateTime.ToString();
                        }
                    }
                    else if (property.Value.PropertyType == EdmType.String)
                    {
                        Console.WriteLine("{0}, {1}", property.Key, property.Value.StringValue);
                        if (property.Key == "LogOutput")
                        {
                            tmp +=":"+ property.Value.StringValue;
                        }
                    }
                    if (tmp != "")
                    {
                        output.Add(tmp);
                    }
                }
            }
            return output;
        }
        /// <summary>
        /// StorageTableより取得したデータをデシリアライズし、レコードをまとめてリストで返します
        /// </summary>
        /// <param name="periodStart"></param>
        /// <param name="periodEnd"></param>
        /// <returns></returns>
        public static List<ContainerAqualiumRawData> DeserializeAqualiumData(DateTimeOffset periodStart, DateTimeOffset periodEnd)
        {
            List<ContainerAqualiumRawData> aquadatas = new List<ContainerAqualiumRawData>();
            List<Exception> except = new List<Exception>();

            List<IDictionary<string, EntityProperty>> tableRecords = AzureStrageTable.getTableRecord(periodStart, periodEnd);

            foreach (var propertys in tableRecords)
            {
                aquadatas.Add(deserializeRecesiveRecord(propertys));
            }
            return aquadatas;
        }
        public static DataSet  DeserializeAqualiumDataSet(DateTimeOffset periodStart, DateTimeOffset periodEnd)
        {
            DataSet output = new DataSet("default_aquaset");

            DataTable data_table = new DataTable("default_aquaTable");
            output.Tables.Add(data_table);



            // データテーブルにカラムを作成・登録
            data_table.Columns.Add("DeviceId", Type.GetType("System.String"));
            data_table.Columns.Add("Timestamp", Type.GetType("System.String"));
            data_table.Columns.Add("Temperature", Type.GetType("System.String"));

            // データテーブルのプライマリーキー（主キー）を設定
            data_table.PrimaryKey = new DataColumn[] { data_table.Columns["col1"] };

            var aquadata =
                from p in DeserializeAqualiumData(periodStart, periodEnd)
                orderby p.Timestamp
                select p;
            foreach (var data in aquadata)
            {
                DataRow data_row;
                // データ行の作成とテーブルへの登録　その１
                data_row = data_table.NewRow();
                data_row["DeviceId"] = data.DeviceId;
                data_row["Timestamp"] = data.Timestamp;
                data_row["Temperature"] = data.Temperature;
                data_table.Rows.Add(data_row);
            }
            return output;
        }
        /// <summary>
        /// 受け取ったIotHubからのレコードを水槽内部データオブジェクトとしてデシリアライズします
        /// </summary>
        /// <param name="propertys"></param>
        /// <returns></returns>
        private static ContainerAqualiumRawData deserializeRecesiveRecord(IDictionary<string, EntityProperty> propertys)
        {
            List<string> element = new List<string>();
            ContainerAqualiumRawData aquaDate = new ContainerAqualiumRawData();
            System.Reflection.PropertyInfo[] propertysContainerAqualiumRawData = typeof(ContainerAqualiumRawData).GetProperties();
            foreach (var property in propertys)
            {
                if (property.Value.PropertyType == EdmType.String)
                {
                    Console.WriteLine("{0}, {1}", property.Key, property.Value.StringValue);
                    //レコード内部のJson文字列で構成されている"LogOutput"をパーシングしオブジェクトへ格納します
                    if (property.Key == "LogOutput")
                    {
                        string logOutput = "";
                        logOutput = property.Value.StringValue.Replace("Message received:", "");
                        logOutput = logOutput.Replace("[", "");
                        logOutput = logOutput.Replace("]", "");
                        logOutput = logOutput.Replace("{", "");
                        logOutput = logOutput.Replace("}", "");
                        foreach(var logstr in logOutput.Split(','))
                        {
                            string[] node = new string[2];
                            node[0] = RevoveChars(logstr.Split(':')[0], new char[] { '"', ':', ' ', '\r', '\n' });
                            node[1] = RevoveChars(logstr.Split(':')[1], new char[] { '"', ':', ' ', '\r', '\n' });
                            foreach (var propertyContainerAqualiumRawData in propertysContainerAqualiumRawData)
                            {
                                if (propertyContainerAqualiumRawData.Name == node[0])
                                {
                                    string tmpstr = node[1];
                                    var tmpValue = propertyContainerAqualiumRawData.GetValue(aquaDate);
                                    var tmpType = tmpValue.GetType();
                                    double tmpD;
                                    int tmpI;

                                    if (tmpType.IsPrimitive)
                                    {
                                        if (double.TryParse(tmpstr, out tmpD))
                                        {
                                            propertyContainerAqualiumRawData.SetValue(aquaDate, tmpD);
                                        }
                                        else if (int.TryParse(tmpstr, out tmpI))
                                        {
                                            propertyContainerAqualiumRawData.SetValue(aquaDate, tmpI);
                                        }
                                        else
                                        {
                                            throw new Exception();
                                        }
                                    }
                                    else if (tmpType.Name == typeof(DateTimeOffset).Name)
                                    {
                                        propertyContainerAqualiumRawData.SetValue(aquaDate, DateTimeOffset.Parse(tmpstr));
                                    }
                                    else
                                    {
                                        propertyContainerAqualiumRawData.SetValue(aquaDate, tmpstr);
                                    }
                                    break;

                                }
                            }
                        }
                    }
                }
                //レコード内部の日時データについては、上記（LogOutput文字列）とは別のパーシングし、対称オブジェクトのプロパティへ格納します
                else if(property.Value.PropertyType == EdmType.DateTime)
                {
                    foreach (var propertyContainerAqualiumRawData in propertysContainerAqualiumRawData)
                    {
                        if (propertyContainerAqualiumRawData.Name == property.Key)
                        {
                            var tmpValue = propertyContainerAqualiumRawData.GetValue(aquaDate);
                            var tmpType = tmpValue.GetType();
                            if (tmpType.Name == typeof(DateTimeOffset).Name)
                            {
                                propertyContainerAqualiumRawData.SetValue(aquaDate, property.Value.DateTimeOffsetValue);
                            }
                        }
                    }
                    
                }
            }
            return aquaDate;
        }
        /// <summary>
        /// 指定した文字列から指定した文字を全て削除する
        /// </summary>
        /// <param name="s">対象となる文字列。</param>
        /// <param name="characters">削除する文字の配列。</param>
        /// <returns>sに含まれている全てのcharacters文字が削除された文字列。</returns>
        public static string RevoveChars(string s, char[] characters)
        {
            System.Text.StringBuilder buf = new System.Text.StringBuilder(s);
            foreach (char c in characters)
            {
                buf.Replace(c.ToString(), "");
            }
            return buf.ToString();
        }
    }
}