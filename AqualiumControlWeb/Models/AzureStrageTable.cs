//参考
//https://docs.microsoft.com/ja-jp/azure/storage/storage-dotnet-how-to-use-tables#a-nameprerequisitesa前提条件
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types
using Newtonsoft.Json;

namespace AqualiumControlWeb.Models
{
    public class AzureStrageTable
    {
        public class aqusaliumRawDate
        {
            public string DeviceId;
            public double Temperature;
            public double Humidity;
            public double ExternalTemperature;
        }



        private static List<IDictionary<string, EntityProperty>> getTableRecord()
        {
            
            List<IDictionary<string, EntityProperty>> output = new List<IDictionary<string, EntityProperty>>();

            
            
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();



            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("AzureWebJobsHostLogs201707");
            OperationContext context = new OperationContext();
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            //TableQuery<aqusaliumRawDate> query = new TableQuery<aqusaliumRawDate>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "I"));

            //ここでテーブルの構造を知らなくてもデータを取ってくることが可能です・・・
            string queryTimeStart = TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThan, DateTimeOffset.Parse("2017-07-07T00:00:00.000Z"));
            string queryTimeEnd = TableQuery.GenerateFilterCondition("StartTime", QueryComparisons.GreaterThan, " 2017-07-01T00:00:00.000Z");
            string queryPatitionKey = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "I");
            string queryWhere=TableQuery.CombineFilters(queryPatitionKey, TableOperators.And, queryTimeStart);
            //TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(queryPatitionKey);
            TableQuery <DynamicTableEntity> query = new TableQuery<DynamicTableEntity>().Where(queryWhere);
             // Print the fields for each customer.
            foreach (var entity in table.ExecuteQuery(query))
            {
                output.Add(entity.Properties);

            }

            return output;
        }

        public static List<string> GetLogDatas()
        {
            List<string> output = new List<string>();

            List<IDictionary<string, EntityProperty>> tableRecords = AzureStrageTable.getTableRecord();

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
        public static void DeserializeAqualiumData()
        {
            List<aqusaliumRawDate> aquadatas = new List<aqusaliumRawDate>();
            List<Exception> except = new List<Exception>();

            List<IDictionary<string, EntityProperty>> tableRecords = AzureStrageTable.getTableRecord();

            foreach (var propertys in tableRecords)
            {
                foreach (var property in propertys)
                {
                    if (property.Value.PropertyType == EdmType.String)
                    {
                        Console.WriteLine("{0}, {1}", property.Key, property.Value.StringValue);
                        if (property.Key == "LogOutput")
                        {
                            string tmp = property.Value.StringValue.Replace("Message received:", "");
                            tmp = tmp.Replace("[", "");
                            tmp = tmp.Replace("]", "");
                            try
                            {
                                aquadatas.Add(JsonConvert.DeserializeObject<aqusaliumRawDate>(tmp));
                            }
                            catch (JsonReaderException err)
                            {
                                except.Add(err);
                            }
                            catch (JsonSerializationException err)
                            {
                                except.Add(err);
                            }
                        }
                    }
                }
            }
        }
    }
}