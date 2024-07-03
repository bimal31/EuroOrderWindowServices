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


public class DA_XmlData : DALBase
{
    public bool INSERT_xmldata(BA_XmlData objBA_XmlData)
    {
        try
        {
            SqlParameter[] p = new SqlParameter[4];
            p[0] = new SqlParameter("@Orderid", objBA_XmlData.Orderid);
            p[1] = new SqlParameter("@OrderXml", objBA_XmlData.OrderXml);
            p[2] = new SqlParameter("@CreateBy", objBA_XmlData.CreateBy);
            p[3] = new SqlParameter("@CreateDate", objBA_XmlData.CreateDate);

            bool flag = this.Execute_NonQuery("sproc_INSERT_OrderXML", p);
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
            return this.Get_Records("sproc_SELECT_ALLOrderXML", ref dt);
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}