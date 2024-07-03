using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;


public class DA_ErrorLog : DALBase
{
    public bool INSERT_ErrorLog(BA_ErrorLog objDA_ErrorLog)
    {
        try
        {
            SqlParameter[] p = new SqlParameter[7];
            p[0] = new SqlParameter("@ERROR_NO", objDA_ErrorLog.ErrorNo);
            p[1] = new SqlParameter("@MESSAGE", objDA_ErrorLog.Message);
            p[2] = new SqlParameter("@DESCRIPTION", objDA_ErrorLog.DESCRIPTION);
            p[3] = new SqlParameter("@URL", objDA_ErrorLog.Url);
            p[4] = new SqlParameter("@STACK_TRACE", objDA_ErrorLog.StackTrace);
            p[5] = new SqlParameter("@IP_ADDED", objDA_ErrorLog.IPAdded);
            p[6] = new SqlParameter("@LAST_IP_UPDATED", objDA_ErrorLog.LAST_IP_UPDATED);
            bool flag = this.Execute_NonQuery("sproc_INSERT_ERROR_LOG", p);
            if (flag)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception )
        {
            return false;
        }
    }


    public bool SELECT_ALL_ErrorLog(ref DataTable dt)
    {
        try
        {
            return this.Get_Records("sproc_SELECT_ALL_ErrorLog", ref dt);
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}