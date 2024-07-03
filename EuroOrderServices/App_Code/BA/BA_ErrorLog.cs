using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public class BA_ErrorLog
{
    DA_ErrorLog objDA_ErrorLog = new DA_ErrorLog();
    public BA_ErrorLog()
    {

    }

    private string _ErrorNo;
    public string ErrorNo
    {
        get { return _ErrorNo; }
        set { _ErrorNo = value; }
    }
    private string _Message;
    public string Message
    {
        get { return _Message; }
        set { _Message = value; }
    }
    private string _DESCRIPTION;
    public string DESCRIPTION
    {
        get { return _DESCRIPTION; }
        set { _DESCRIPTION = value; }
    }
    private string _Url;
    public string Url
    {
        get { return _Url; }
        set { _Url = value; }
    }
    private string _StackTrace;
    public string StackTrace
    {
        get { return _StackTrace; }
        set { _StackTrace = value; }
    }
    private string _IPAdded;
    public string IPAdded
    {
        get { return _IPAdded; }
        set { _IPAdded = value; }
    }
    private string _LAST_IP_UPDATED;
    public string LAST_IP_UPDATED
    {
        get { return _LAST_IP_UPDATED; }
        set { _LAST_IP_UPDATED = value; }
    }

    public bool INSERT_ErrorLog(Exception exce,string url)
    {
        try
        {

            BA_ErrorLog ObjError = new BA_ErrorLog();
            ObjError.ErrorNo = "0";
            ObjError.Message = exce.Message;
            ObjError.DESCRIPTION = exce.Message;
            ObjError.Url = url;
            ObjError.StackTrace = exce.StackTrace;
            ObjError.IPAdded = Common.GetLocalIPAddress();
            ObjError.LAST_IP_UPDATED = Common.GetLocalIPAddress();

            return objDA_ErrorLog.INSERT_ErrorLog(ObjError);
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
            if (objDA_ErrorLog.SELECT_ALL_ErrorLog( ref dt))
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