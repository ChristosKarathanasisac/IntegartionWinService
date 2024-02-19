using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationWinService
{
    class IntegrationBuilder
    {
        private string _dataSource = @"//rep_DataSource";
        private string _initialcatalog = @"//rep_Initialcatalog";
        private string _url = "url";
        public void StartUpdate() 
        {
            try 
            { 
                string sqlQuery = "rep_sqlquery";
                string error = "";
                DataTable dt = GeneralUtils.GetDataTable(sqlQuery, _dataSource, _initialcatalog, out error);

                IntegrationData data = MappingFun(dt);
                string json = GeneralUtils.CreateJsonString(data);

                string repsonse = GeneralUtils.SendRequestPostNew(_url, out error, json);
               
            }
            catch (Exception exc) 
            {
                //Exception message
            }
        }

       //rep_code1
    }
}
