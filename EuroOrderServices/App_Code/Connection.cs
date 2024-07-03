using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace EuroOrderServices
{
     public static class Connection
    {
        public static string GetSConnectionString()
        {
            try
            {
                return Convert.ToString(ConfigurationManager.AppSettings["ConnectionString"]);
            }
            catch (Exception ex)
            {
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "GetSConnectionString");
                return "";
            }
            
        }




    }
}
