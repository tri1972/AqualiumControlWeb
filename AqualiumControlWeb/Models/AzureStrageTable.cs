//参考
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
        private static readonly string AZURE_LOG_NAME = "AzureWebJobsHostLogs";
        private static List<IDictionary<string, EntityProperty>> getTableRecord(DateTimeOffset periodStart, DateTimeOffset periodEnd)
        {

            List<IDictionary<string, EntityProperty>> output = new List<IDictionary<string, EntityProperty>>();

            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            if (periodEnd.Year-periodStart.Year <2 )
            {
            }
            else
            {
                throw new Exception("2年をまたいでログの取得はできません");
            }
            for (int j = 0; j < periodEnd.Year - periodStart.Year + 1; j++)
            {
                string year = (periodStart.Year+j).ToString();
                int startMonth, endMonth;
                if(periodEnd.Year - periodStart.Year < 1)
                {
                    startMonth = periodStart.Month;
                    endMonth = periodEnd.Month;
                }
                else
                {
                    if (j == 0)
                    {
                        startMonth = periodStart.Month;
                        endMonth = 12;
                    }
                    else
                    {
                        startMonth = 1;
                        endMonth = periodEnd.Month;

                    }
                }
                for (int i = 0; i < (endMonth - startMonth) + 1; i++)
                {
                    string month;
                    if ((startMonth + i) < 10)
                    {
                        month = "0" + (startMonth + i).ToString();
                    }
                    else
                    {
                        month = (startMonth + i).ToString();
                    }
                    // Create the CloudTable object that represents the "people" table.
                    CloudTable table = tableClient.GetTableReference(AZURE_LOG_NAME + year + month);
                    // Construct the query operation for all customer entities where PartitionKey="Smith".
                    //TableQuery<aqusaliumRawDate> query = new TableQuery<aqusaliumRawDate>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "I"));

                    output.AddRange(getMonthTableRecord(periodStart, periodEnd, table));

                }
            }
            return output;
        }

        /// <summary>
        /// 現時点に一番近いレコードを取得します
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, EntityProperty> getTableRecordCurrent()
        {
            var periodStart = DateTime.Now;

            var output = new Dictionary<string, EntityProperty>();

            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            string year = (periodStart.Year).ToString();
            int startMonth = periodStart.Month;
            string month;
            if ((startMonth) < 10)
            {
                month = "0" + (startMonth).ToString();
            }
            else
            {
                month = (startMonth).ToString();
            }
            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(AZURE_LOG_NAME + year + month);
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            //TableQuery<aqusaliumRawDate> query = new TableQuery<aqusaliumRawDate>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "I"));

            return getTableCurrentRecord(periodStart, table);
        }
        /// <summary>
        /// AzureIOTのテーブルよりレコードを取得します。ただし月を超えて取得することは不可能です
        /// </summary>
        /// <param name="periodStart"></param>
        /// <param name="periodEnd"></param>
        /// <param name="output"></param>
        /// <param name="table"></param>
        private static List<IDictionary<string, EntityProperty>> getMonthTableRecord(DateTimeOffset periodStart, DateTimeOffset periodEnd, CloudTable table)
        {
            var output = new List<IDictionary<string, EntityProperty>>();
            //ここでテーブルの構造を知らなくてもデータを取ってくることが可能です・・・

            //var localStart = new DateTimeOffset(periodStart.LocalDateTime.AddHours(9.0));
            //var localEnd = new DateTimeOffset(periodEnd.LocalDateTime.AddHours(9.0));
            //IOTのレコード時間はUTCらしいのでそのまま検索します
            var localStart = periodStart;
            var localEnd= periodEnd;

            string queryPeriodStart = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, localStart);
            string queryPeriodEnd = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThanOrEqual, localEnd);
            string queryPatitionKey = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "I");
            string queryPeriod = TableQuery.CombineFilters(queryPeriodStart, TableOperators.And, queryPeriodEnd);
            string queryWhere = TableQuery.CombineFilters(queryPatitionKey, TableOperators.And, queryPeriod);

            //TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(queryPatitionKey);
            TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(queryWhere);
            
            // Print the fields for each customer.
            foreach (var entity in table.ExecuteQuery(query))
            {//めんどくさいのでpropertiesにTimeStampも追加してしまいます
                entity.Properties.Add("Timestamp", new EntityProperty(entity.Timestamp));
                //また、レコードの時間についてはローカルタイムに変換します
                //entity.Properties.Add("Timestamp", new EntityProperty(new DateTimeOffset(entity.Timestamp.LocalDateTime.AddHours(9.0))));
                output.Add(entity.Properties);

            }
            return output;
        }
        /// <summary>
        /// 指定した時刻に一番近いデータを取得します
        /// </summary>
        /// <param name="periodNow">時刻</param>
        /// <param name="table">取得したいテーブル</param>
        /// <returns></returns>
        private static Dictionary<string, EntityProperty> getTableCurrentRecord(DateTimeOffset periodNow, CloudTable table)
        {
            var output = new Dictionary<string, EntityProperty>();
            //ここでテーブルの構造を知らなくてもデータを取ってくることが可能です・・・

            //var localStart = new DateTimeOffset(periodStart.LocalDateTime.AddHours(9.0));
            //IOTのレコード時間はUTCらしいのでそのまま検索します
            var localStart = periodNow;

            string queryPeriodStart = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThanOrEqual, localStart);
            string queryPatitionKey = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "I");
            string queryWhere = TableQuery.CombineFilters(queryPatitionKey, TableOperators.And, queryPeriodStart);

            //TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(queryPatitionKey);
            TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(queryWhere);

            var maxTime = new DateTimeOffset();
            // Print the fields for each customer.
            foreach (var entity in table.ExecuteQuery(query))
            {
                if (maxTime <= entity.Timestamp)
                {
                    //めんどくさいのでpropertiesにTimeStampも追加してしまいます
                    entity.Properties.Add("Timestamp", new EntityProperty(entity.Timestamp));
                    //また、レコードの時間についてはローカルタイムに変換します
                    //entity.Properties.Add("Timestamp", new EntityProperty(new DateTimeOffset(entity.Timestamp.LocalDateTime.AddHours(9.0))));
                    output = entity.Properties as Dictionary<string, EntityProperty>;
                    maxTime = entity.Timestamp;
                }
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
        public static List<ContainerAqualiumRawData> DeserializeAqualiumDatas(DateTimeOffset periodStart, DateTimeOffset periodEnd)
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

        /// <summary>
        ///　現在の環境データを取得します 
        /// </summary>
        /// <returns></returns>
        public static ContainerAqualiumRawData DeserializeAqualiumDataCurrent()
        {

            var aquadatas = new ContainerAqualiumRawData();

            var tableRecords = AzureStrageTable.getTableRecordCurrent();

            return deserializeRecesiveRecord(tableRecords);
        }

        public static DataSet  DeserializeAqualiumDataSet(DateTimeOffset periodStart, DateTimeOffset periodEnd)
        {
            DatasetAqualium output;

            output = new DatasetAqualium("aqua");
            output.Initialize();

            //InitDatasetAqualium(out output, out data_table);

            var aquadata =
                from p in DeserializeAqualiumDatas(periodStart, periodEnd)
                orderby p.Timestamp
                select p;
            foreach (var data in aquadata)
            {
                DataRow data_row;
                // データ行の作成とテーブルへの登録　その１
                data_row = output.TableAqua.NewRow();
                data_row["DeviceId"] = data.DeviceId;
                data_row["Timestamp"] = data.Timestamp;
                data_row["Temperature"] = data.Temperature;
                data_row["Humidity"] = data.Humidity;
                data_row["ExternalTemperature"] = data.ExternalTemperature;
                data_row["Pressure"] = data.Pressure;

                output.TableAqua.Rows.Add(data_row);
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