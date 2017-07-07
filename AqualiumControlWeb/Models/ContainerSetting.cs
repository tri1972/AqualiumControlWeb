using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AqualiumControlWeb.Models
{
    /// <summary>
    /// 初期設定データを保存します（データはWeb.configを上書きする、settings.configに保存されます）
    /// </summary>
    public class ContainerSetting
    {

        private string storageConnectionString;
        /// <summary>
        /// AzureStorage接続用文字列を取得または設定します
        /// </summary>
        public string StorageConnectionString
        {
            get { return this.storageConnectionString; }
            set { this.storageConnectionString = value; }
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ContainerSetting()
        {

        }
    }
}