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
using Oracle.ManagedDataAccess.Client;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using HIS_DB_Lib;
namespace DB2VM_API
{
   

    [Route("api/[controller]")]
    [ApiController]
    public class OutTakeMedController
    {
        [Route("Sample")]
        [HttpGet()]
        public string Get_Sample()
        {
            string str = Basic.Net.WEBApiGet(@"http://10.14.16.50:443/api/OutTakeMed/Sample");

            return str;
        }
        [HttpPost]
        public string Post([FromBody] List<class_OutTakeMed_data> data)
        {
            if (data.Count == 0) return "";
            string json = data.JsonSerializationt();

            for (int i = 0; i < data.Count; i++)
            {
                string code = data[i].藥品碼;
                medClass medClass = medClass.get_med_clouds_by_code("http:/127.0.0.1:4433", code);
                if (medClass != null)
                {
                    data[i].藥名 = medClass.藥品名稱;
                    data[i].單位 = medClass.包裝單位;
                }
            }
            string str = "";
            if(data[0].成本中心 == "1")
            {
                str = Basic.Net.WEBApiPostJson("http://10.14.16.50:443/api/OutTakeMed/", json);
            }
            if (data[0].成本中心 == "2")
            {
                str = Basic.Net.WEBApiPostJson("http://10.14.16.49:443/api/OutTakeMed/", json);
            }
            return str;
        }
    }
}
