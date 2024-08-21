using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBM.Data.DB2.Core;
using System.Data;
using System.Configuration;
using Basic;
using SQLUI;
using System.Xml;
using System.Text.Json;
using System.Text.Json.Serialization;
using HIS_DB_Lib;
namespace DB2VM.Controller
{
    [Route("dbvm/[controller]")]
    [ApiController]
    public class BBCMController : ControllerBase
    { 
        public class class_BBCM_data
        {
            [JsonPropertyName("MEDCODE")]
            public string 藥品碼 { get; set; }
            [JsonPropertyName("MEDNAME")]
            public string 藥品名稱 { get; set; }
            [JsonPropertyName("ENGNAME")]
            public string 藥品學名 { get; set; }
            [JsonPropertyName("CHTNAME")]
            public string 中文名稱 { get; set; }
            [JsonPropertyName("EASYNAME")]
            public string 簡名 { get; set; }
            [JsonPropertyName("PKG")]
            public string 包裝單位 { get; set; }
            [JsonPropertyName("ISEMG")]
            public string 警訊藥品 { get; set; }
            [JsonPropertyName("DRUGKIND")]
            public string 類別 { get; set; }
            [JsonPropertyName("RESTRIC")]
            public string 管制級別 { get; set; }
        
        }
        static string MySQL_server = $"{ConfigurationManager.AppSettings["MySQL_server"]}";
        static string MySQL_database = $"{ConfigurationManager.AppSettings["MySQL_database"]}";
        static string MySQL_userid = $"{ConfigurationManager.AppSettings["MySQL_user"]}";
        static string MySQL_password = $"{ConfigurationManager.AppSettings["MySQL_password"]}";
        static string MySQL_port = $"{ConfigurationManager.AppSettings["MySQL_port"]}";

        private SQLControl sQLControl_藥檔資料 = new SQLControl(MySQL_server, MySQL_database, "medicine_page_cloud", MySQL_userid, MySQL_password, (uint)MySQL_port.StringToInt32(), MySql.Data.MySqlClient.MySqlSslMode.None);


        [HttpGet]
        public string Get(string Code)
        {
            List<string> codes = new List<string>();
            List<class_BBCM_data> class_BBCM_Datas = new List<class_BBCM_data>();
            if(Code.StringIsEmpty() == false)
            {
                codes.Add(Code);
            }
            string PostString = "";
            if (codes.Count > 0) PostString = codes.JsonSerializationt();
            string jsonstring = Basic.Net.WEBApiPostJson("https://phpsrv.tahsda.org.tw/api/medicine_page.php", PostString);
            //jsonstring = System.Text.RegularExpressions.Regex.Unescape(jsonstring);
            class_BBCM_Datas = jsonstring.JsonDeserializet<List<class_BBCM_data>>();
            List<object[]> list_藥檔資料 = sQLControl_藥檔資料.GetAllRows(null);
            List<object[]> list_藥檔資料_buf = new List<object[]>();
            List<object[]> list_藥檔資料_add = new List<object[]>();
            List<object[]> list_藥檔資料_replace = new List<object[]>();
            for (int i = 0; i < class_BBCM_Datas.Count; i++)
            {
                string 藥品碼 = class_BBCM_Datas[i].藥品碼;
                string 藥品名稱 = class_BBCM_Datas[i].藥品名稱;
                string 藥品學名 = class_BBCM_Datas[i].藥品學名;
                string 中文名稱 = class_BBCM_Datas[i].中文名稱;
                string 包裝單位 = class_BBCM_Datas[i].包裝單位;
                if (class_BBCM_Datas[i].警訊藥品 == "Y")
                {
                    class_BBCM_Datas[i].警訊藥品 = "True";
                }
                else
                {
                    class_BBCM_Datas[i].警訊藥品 = "False";
                }
                string 警訊藥品 = class_BBCM_Datas[i].警訊藥品;
                string 類別 = class_BBCM_Datas[i].類別;
              
                string 管制級別 = class_BBCM_Datas[i].管制級別;

                list_藥檔資料_buf = list_藥檔資料.GetRows((int)enum_雲端藥檔.藥品碼, 藥品碼);
                if (list_藥檔資料_buf.Count == 0)
                {
                    object[] value = new object[new enum_雲端藥檔().GetLength()];
                    value[(int)enum_雲端藥檔.GUID] = Guid.NewGuid().ToString();
                    value[(int)enum_雲端藥檔.藥品碼] = 藥品碼;
                    value[(int)enum_雲端藥檔.藥品名稱] = 藥品名稱;
                    value[(int)enum_雲端藥檔.中文名稱] = 中文名稱;
                    value[(int)enum_雲端藥檔.藥品學名] = 藥品學名;
                    value[(int)enum_雲端藥檔.警訊藥品] = 警訊藥品;
                    value[(int)enum_雲端藥檔.包裝單位] = 包裝單位;
                    value[(int)enum_雲端藥檔.類別] = 類別;
                    value[(int)enum_雲端藥檔.管制級別] = 管制級別;
                    list_藥檔資料_add.Add(value);
                }
                else
                {
                    object[] value = list_藥檔資料_buf[0];
                    value[(int)enum_雲端藥檔.藥品碼] = 藥品碼;
                    value[(int)enum_雲端藥檔.藥品名稱] = 藥品名稱;
                    value[(int)enum_雲端藥檔.中文名稱] = 中文名稱;
                    value[(int)enum_雲端藥檔.藥品學名] = 藥品學名;
                    value[(int)enum_雲端藥檔.警訊藥品] = 警訊藥品;
                    value[(int)enum_雲端藥檔.包裝單位] = 包裝單位;
                    value[(int)enum_雲端藥檔.類別] = 類別;
                    value[(int)enum_雲端藥檔.管制級別] = 管制級別;
                    list_藥檔資料_replace.Add(value);
                }
            }
            if(list_藥檔資料_add.Count > 0)
            {
                sQLControl_藥檔資料.AddRows(null, list_藥檔資料_add);
            }
            if (list_藥檔資料_replace.Count > 0)
            {
                sQLControl_藥檔資料.UpdateByDefulteExtra(null, list_藥檔資料_replace);
            }
            return class_BBCM_Datas.JsonSerializationt(true); 
        }
    }
}
