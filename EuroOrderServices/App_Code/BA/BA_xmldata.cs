using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public class BA_XmlData
{
    DA_XmlData objDA_XmlData = new DA_XmlData();
    public BA_XmlData()
    {

    }

    private long _Orderid;
    public long Orderid
    {
        get { return _Orderid; }
        set { _Orderid = value; }
    }
    private string _OrderXml;
    public string OrderXml
    {
        get { return _OrderXml; }
        set { _OrderXml = value; }
    }

    private long _CreateBy;
    public long CreateBy
    {
        get { return _CreateBy; }
        set { _CreateBy = value; }
    }


    private DateTime _CreateDate;
    public DateTime CreateDate
    {
        get { return _CreateDate; }
        set { _CreateDate = value; }
    }


    public bool INSERT_XmlData(string  xml,long orderid)
    {
        try
        {

            BA_XmlData ObjBA_XmlData = new BA_XmlData();
            ObjBA_XmlData.Orderid = orderid;
            ObjBA_XmlData.OrderXml = xml;
            ObjBA_XmlData.CreateBy = 0;
            ObjBA_XmlData.CreateDate = DateTime.Now;
           
            return objDA_XmlData.INSERT_xmldata(ObjBA_XmlData);
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public bool SELECT_ALL_ErrorLog(ref DataTable dt)
    {
        try
        {
            if (objDA_XmlData.SELECT_ALL_ErrorLog( ref dt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}