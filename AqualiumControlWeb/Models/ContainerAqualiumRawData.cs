using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AqualiumControlWeb.Models
{
    /// <summary>
    /// StrageTableよりとってきたデータを格納するクラスです
    /// </summary>
    public class ContainerAqualiumRawData
    {
        private string deviceId;

        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceId
        {
            get { return this.deviceId; }
            set { this.deviceId = value; }
        }
        private double temperature;
        /// <summary>
        /// 温度
        /// </summary>
        public double Temperature
        {
            get { return this.temperature; }
            set { this.temperature = value; }
        }
        private double humidity;
        /// <summary>
        /// 湿度
        /// </summary>
        public double Humidity
        {
            get { return this.humidity; }
            set { this.humidity = value; }
        }
        private double externalTemperature;
        /// <summary>
        /// 室温
        /// </summary>
        public double ExternalTemperature
        {
            get { return this.externalTemperature; }
            set { this.externalTemperature = value; }
        }
        private double pressure;
        /// <summary>
        /// 圧力
        /// </summary>
        public double Pressure
        {
            get { return this.pressure; }
            set { this.pressure = value; }
        }
        private DateTimeOffset timestamp;
        /// <summary>
        /// タイムスタンプ
        /// </summary>
        public DateTimeOffset Timestamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ContainerAqualiumRawData()
        {
            timestamp = new DateTimeOffset();
            deviceId = "";
        } 
    }
}