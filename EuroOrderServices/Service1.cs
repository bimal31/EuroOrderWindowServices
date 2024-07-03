using EuroOrderServices.App_Code;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;
using static EuroOrderServices.App_Code.TallyCls;

namespace EuroOrderServices
{
    public partial class EuroOrderServices : ServiceBase
    {
        private const bool IsCreadReq = true;
        public string ConnectionString { get; set; }
        Timer timer = new Timer();

        public EuroOrderServices()
        {
            try
            {
                InitializeComponent();
                //getdataodbc();
              //  GetPendingdata();
                // GetFactoryData();
            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("EuroOrderServices " + ex.ToString());
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "EuroOrderServices");
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                ServiceLog.WriteErrorLog("Service is started at " + DateTime.Now);
                timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
                timer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["IntervalTime"].ToString()); //number in milisecinds  
                timer.Enabled = true;
                timer.Start();
            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("OnStart " + ex.ToString());
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "OnStart");
            }
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            ServiceLog.WriteErrorLog("Service is stopped at " + DateTime.Now);
        }


        #region OnElapsedTime
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                ServiceLog.WriteErrorLog("OnElapsedTime " + DateTime.Now);
                GetPendingdata();
                GetFactoryData(); 

            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("OnElapsedTime error  " + ex.ToString());
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "OnElapsedTime");
            }
        }

        public void GetPendingdata()
        {
            try
            {
                var rmsConnectionString = Connection.GetSConnectionString();
                ConnectionString = rmsConnectionString;
                DataSet odata = SqlHelper.ExecuteDataset(rmsConnectionString, CommandType.StoredProcedure, "Proc_GetpendingpaymentCall",
                 new SqlParameter("@daydate", DateTime.Now));

                if (odata.Tables[0].Rows.Count ==0)
                {
                    getdataodbc();
                }

            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("GetPendingdata error = " + ex.ToString());
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "GetPendingdata");
            }
        }
        public void getdataodbc()
        {
            try
            {
                string query = "SELECT JRPartyOSTbl.`$DCode`,JRPartyOSTbl.`$DispNm`, JRPartyOSTbl.`$GetTotDueAmt` FROM TallyUser.JRPartyOSTbl JRPartyOSTbl";
                string source = "DRIVER=Tally ODBC Driver64;SERVER=(local);PORT=9001;";
                OdbcConnection con = new OdbcConnection(source);
                OdbcCommand cmd = new OdbcCommand(query, con);
                con.Open();
                OdbcDataAdapter da = new OdbcDataAdapter(query, con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                con.Close();

                string DealerCode = "";
                if (ds.Tables[0] != null)
                {
                    string strAddedBarcodeData = string.Empty;
                    StringBuilder strAddBarcodeXML = new StringBuilder();
                    strAddBarcodeXML.AppendFormat("<root>");
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DealerCode = Convert.ToString(ds.Tables[0].Rows[i]["JRPartyOSTbl.`$DCode`"]);
                        if (DealerCode != "")
                        {
                            strAddBarcodeXML.AppendFormat("<PendingDealer  DealerCode=''" + Convert.ToString(ds.Tables[0].Rows[i]["JRPartyOSTbl.`$DCode`"]) +
                                "'' DealerName=''" + Convert.ToString(ds.Tables[0].Rows[i]["JRPartyOSTbl.`$DCode`"]) +
                                "'' AMOUNT=''" + Convert.ToDecimal(ds.Tables[0].Rows[i]["JRPartyOSTbl.`$GetTotDueAmt`"]) + "''  />");
                        }
                    }
                    strAddBarcodeXML.AppendFormat("</root>");
                    strAddedBarcodeData = strAddBarcodeXML.ToString().Replace("''", "\"");

                    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "InsertPendingDelaerCode",
                    new SqlParameter("@PedningXML", strAddedBarcodeData));

                }
            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("getdataodbc  " + ex.ToString());
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "getdataodbc");
            }
        }
        #endregion

        public void GetFactoryData()
        {
            bool IsSucess;
            int OrderID = 0;
            string xmlPass = "";
            try
            {
               
                DataSet DsData = GetTallyData();
                if (DsData != null && DsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < DsData.Tables[0].Rows.Count; i++)
                    {
                        OrderID = Convert.ToInt32(DsData.Tables[0].Rows[i]["OrderID"]);
                        DataSet DsDataOrder = PostTallyData(OrderID);
                        ServiceLog.WriteErrorLog("Service is OrderID " + OrderID);
                        xmlPass = xmldataread(DsDataOrder, OrderID);
                        if (xmlPass != "")
                        {
                            IsSucess = false;
                            ServiceLog.WriteErrorLog("Service SendTallyRequest");
                            IsSucess = SendTallyRequest(xmlPass);
                            if (IsSucess)
                            {
                                UpdateImportTallyData(OrderID);
                            }
                            else
                            {
                                sendmail(DsDataOrder);
                            }
                        }
                        else
                        {
                            sendmail(DsDataOrder);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("OnElapsedTime error  " + ex.ToString());
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "getdate");
            }
        }

        public string xmldataread(DataSet ds, long OrderID)
        {
            try
            {

                string guid = "", billdate = "", billstate = "", PARTYGSTIN = "", PARTYNAME = "", PARTYPINCODE = "";
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    guid = Convert.ToString(ds.Tables[0].Rows[0]["OrderSrNo"]) + Convert.ToDateTime(ds.Tables[0].Rows[0]["OrderDate"]).ToString("yyyyMMddHHmmss");
                    billdate = Convert.ToDateTime(ds.Tables[0].Rows[0]["OrderDate"]).ToString("yyyyMMdd");
                    billstate = Convert.ToString(ds.Tables[0].Rows[0]["state_name"]);
                    PARTYGSTIN = Convert.ToString(ds.Tables[0].Rows[0]["GST"]);
                    PARTYNAME = Convert.ToString(ds.Tables[0].Rows[0]["DealerCode"]);
                    PARTYPINCODE = Convert.ToString(ds.Tables[0].Rows[0]["Pincode"]);
                    if (PARTYPINCODE == "")
                    {
                        PARTYPINCODE = "0";
                    }
                }
                ENVELOPE objENVELOPE = new ENVELOPE();

                #region Header
                TallyCls.HEADER objHEADER = new TallyCls.HEADER();
                objHEADER.TALLYREQUEST = Convert.ToString(ConfigurationManager.AppSettings["TALLYREQUEST"]);

                objENVELOPE.HEADER = objHEADER;
                #endregion

                #region BODY

                TallyCls.BODY objBODY = new TallyCls.BODY();
                TallyCls.IMPORTDATA objIMPORTDATA = new TallyCls.IMPORTDATA();



                #region REQUESTDESC
                TallyCls.REQUESTDESC objREQUESTDESC = new TallyCls.REQUESTDESC();

                objREQUESTDESC.REPORTNAME = Convert.ToString(ConfigurationManager.AppSettings["REPORTNAME"]);

                TallyCls.STATICVARIABLES objSTATICVARIABLES = new TallyCls.STATICVARIABLES();
                objSTATICVARIABLES.SVCURRENTCOMPANY = Convert.ToString(ConfigurationManager.AppSettings["SVCURRENTCOMPANY"]);

                objREQUESTDESC.STATICVARIABLES = objSTATICVARIABLES;

                objIMPORTDATA.REQUESTDESC = objREQUESTDESC;
                #endregion

                //#region REQUESTDATA
                TallyCls.REQUESTDATA objREQUESTDATA = new TallyCls.REQUESTDATA();

                TallyCls.TALLYMESSAGE objTALLYMESSAGE = new TallyCls.TALLYMESSAGE();
                objTALLYMESSAGE.UDF = Convert.ToString(ConfigurationManager.AppSettings["UDF"]);

                TallyCls.VOUCHER objVOUCHER = new TallyCls.VOUCHER();

                objVOUCHER.REMOTEID = "";
                objVOUCHER.VCHKEY = "";
                objVOUCHER.SENDERID = guid;
                objVOUCHER.VCHTYPE = Convert.ToString(ConfigurationManager.AppSettings["VCHTYPE"]);
                objVOUCHER.ACTION = Convert.ToString(ConfigurationManager.AppSettings["ACTION"]);
                objVOUCHER.OBJVIEW = Convert.ToString(ConfigurationManager.AppSettings["OBJVIEW"]);

                TallyCls.ADDRESSLIST OBJADDRESSLIST = new TallyCls.ADDRESSLIST();
                List<string> ADDRESSvalue = new List<string>();
                ADDRESSvalue.Add(Convert.ToString(ds.Tables[0].Rows[0]["Address"]));
                ADDRESSvalue.Add(Convert.ToString(ds.Tables[0].Rows[0]["Area"]));
                ADDRESSvalue.Add(Convert.ToString(ds.Tables[0].Rows[0]["Pincode"]));
                OBJADDRESSLIST.Text = Convert.ToString(ConfigurationManager.AppSettings["OBJADDRESSLIST_TEXT"]);

                OBJADDRESSLIST.ADDRESS = ADDRESSvalue;
                //OBJADDRESSLIST.TYPE = "String";
                objVOUCHER.ADDRESSLIST = OBJADDRESSLIST;

                TallyCls.DISPATCHFROMADDRESSLIST objDISPATCHFROMADDRESSLIST = new TallyCls.DISPATCHFROMADDRESSLIST();
                objDISPATCHFROMADDRESSLIST.TYPE = Convert.ToString(ConfigurationManager.AppSettings["DISPATCHFROMADDRESSLIST_TEXT"]);
                List<string> dispADDRESSvalue = new List<string>();
                dispADDRESSvalue.Add(Convert.ToString(ConfigurationManager.AppSettings["DISPATCHFROMADDRESS1"]));
                dispADDRESSvalue.Add(Convert.ToString(ConfigurationManager.AppSettings["DISPATCHFROMADDRESS2"]));
                dispADDRESSvalue.Add(Convert.ToString(ConfigurationManager.AppSettings["DISPATCHFROMADDRESS3"]));

                objDISPATCHFROMADDRESSLIST.DISPATCHFROMADDRESS = dispADDRESSvalue;
                objVOUCHER.DISPATCHFROMADDRESSLIST = objDISPATCHFROMADDRESSLIST;

                TallyCls.BASICBUYERADDRESSLIST objBASICBUYERADDRESSLIST = new TallyCls.BASICBUYERADDRESSLIST();
                objBASICBUYERADDRESSLIST.TYPE = Convert.ToString(ConfigurationManager.AppSettings["BASICBUYERADDRESSLIST_TEXT"]);
                List<string> BASICBUYERADDRESSValue = new List<string>();

                BASICBUYERADDRESSValue.Add(Convert.ToString(ds.Tables[0].Rows[0]["Address"]));
                BASICBUYERADDRESSValue.Add(Convert.ToString(ds.Tables[0].Rows[0]["Area"]));
                BASICBUYERADDRESSValue.Add(Convert.ToString(ds.Tables[0].Rows[0]["Pincode"]));
                objBASICBUYERADDRESSLIST.BASICBUYERADDRESS = BASICBUYERADDRESSValue;
                objVOUCHER.BASICBUYERADDRESSLIST = objBASICBUYERADDRESSLIST;

                TallyCls.OLDAUDITENTRYIDSLIST objOLDAUDITENTRYIDSLIST = new TallyCls.OLDAUDITENTRYIDSLIST();
                objOLDAUDITENTRYIDSLIST.TYPE = Convert.ToString(ConfigurationManager.AppSettings["OLDAUDITENTRYIDSLIST"]);
                objOLDAUDITENTRYIDSLIST.OLDAUDITENTRYIDS = Convert.ToInt32(ConfigurationManager.AppSettings["OLDAUDITENTRYIDS"]); // - 1;
                objVOUCHER.OLDAUDITENTRYIDSLIST = objOLDAUDITENTRYIDSLIST;

                objVOUCHER.DATE = Convert.ToInt32(billdate);
                objVOUCHER.IRNACKDATE = Convert.ToInt32(billdate);
                objVOUCHER.GUID = guid;
                objVOUCHER.GSTREGISTRATIONTYPE = Convert.ToString(ConfigurationManager.AppSettings["GSTREGISTRATIONTYPE"]);
                objVOUCHER.VATDEALERTYPE = Convert.ToString(ConfigurationManager.AppSettings["VATDEALERTYPE"]);
                objVOUCHER.STATENAME = billstate;
                objVOUCHER.COUNTRYOFRESIDENCE = Convert.ToString(ConfigurationManager.AppSettings["COUNTRYOFRESIDENCE"]);
                objVOUCHER.PARTYGSTIN = PARTYGSTIN;

                objVOUCHER.PLACEOFSUPPLY = billstate;
                objVOUCHER.CLASSNAME = "";
                objVOUCHER.PARTYNAME = PARTYNAME;
                objVOUCHER.PARTYLEDGERNAME = PARTYNAME;
                objVOUCHER.VOUCHERTYPENAME = Convert.ToString(ConfigurationManager.AppSettings["VOUCHERTYPENAME"]);
                objVOUCHER.PARTYMAILINGNAME = PARTYNAME;

                objVOUCHER.PARTYPINCODE = Convert.ToInt32(PARTYPINCODE);
                objVOUCHER.CONSIGNEEGSTIN = PARTYGSTIN;
                objVOUCHER.CONSIGNEEMAILINGNAME = PARTYNAME;
                objVOUCHER.CONSIGNEEPINCODE = Convert.ToInt32(PARTYPINCODE);
                objVOUCHER.CONSIGNEESTATENAME = billstate;

                #region ADD Voucher no missing
                objVOUCHER.VOUCHERNUMBER = Convert.ToString(ds.Tables[0].Rows[0]["OrderID"]);
                objVOUCHER.Reference = Convert.ToString(ds.Tables[0].Rows[0]["OrderID"]);
                #endregion

                objVOUCHER.BASICBASEPARTYNAME = PARTYNAME;
                objVOUCHER.CSTFORMISSUETYPE = "";
                objVOUCHER.CSTFORMRECVTYPE = "";
                objVOUCHER.FBTPAYMENTTYPE = Convert.ToString(ConfigurationManager.AppSettings["FBTPAYMENTTYPE"]);

                objVOUCHER.PERSISTEDVIEW = Convert.ToString(ConfigurationManager.AppSettings["PERSISTEDVIEW"]);
                objVOUCHER.BASICBUYERNAME = PARTYNAME;
                objVOUCHER.CONSIGNEECOUNTRYNAME = Convert.ToString(ConfigurationManager.AppSettings["CONSIGNEECOUNTRYNAME"]);
                objVOUCHER.VCHGSTCLASS = "";
                objVOUCHER.VCHENTRYMODE = Convert.ToString(ConfigurationManager.AppSettings["VCHENTRYMODE"]);

                objVOUCHER.VOUCHERTYPEORIGNAME = Convert.ToString(ConfigurationManager.AppSettings["VOUCHERTYPEORIGNAME"]);
                objVOUCHER.DIFFACTUALQTY = Convert.ToString(ConfigurationManager.AppSettings["DIFFACTUALQTY"]);
                objVOUCHER.ISMSTFROMSYNC = Convert.ToString(ConfigurationManager.AppSettings["ISMSTFROMSYNC"]);
                objVOUCHER.ASORIGINAL = Convert.ToString(ConfigurationManager.AppSettings["ASORIGINAL"]);
                objVOUCHER.AUDITED = Convert.ToString(ConfigurationManager.AppSettings["AUDITED"]);

                objVOUCHER.FORJOBCOSTING = Convert.ToString(ConfigurationManager.AppSettings["FORJOBCOSTING"]);
                objVOUCHER.ISOPTIONAL = Convert.ToString(ConfigurationManager.AppSettings["ISOPTIONAL"]);
                objVOUCHER.EFFECTIVEDATE = Convert.ToInt32(billdate);
                objVOUCHER.ISINVOICE = Convert.ToString(ConfigurationManager.AppSettings["ISINVOICE"]);


                #region ALLINVENTORYENTRIES.LIST
                List<TallyCls.ALLINVENTORYENTRIESLIST> objALLINVENTORYENTRIESLIST = new List<TallyCls.ALLINVENTORYENTRIESLIST>();
                if (ds != null && ds.Tables[1].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {

                        TallyCls.ALLINVENTORYENTRIESLIST objALLINVENTORYENTRIES = new TallyCls.ALLINVENTORYENTRIESLIST();

                        objALLINVENTORYENTRIES.STOCKITEMNAME = Convert.ToString(ds.Tables[1].Rows[i]["ProductCode"]);
                        objALLINVENTORYENTRIES.ISDEEMEDPOSITIVE = Convert.ToString(ConfigurationManager.AppSettings["ISDEEMEDPOSITIVE"]);
                        objALLINVENTORYENTRIES.ISLASTDEEMEDPOSITIVE = Convert.ToString(ConfigurationManager.AppSettings["ISLASTDEEMEDPOSITIVE"]);
                        objALLINVENTORYENTRIES.RATE = Convert.ToInt32(ConfigurationManager.AppSettings["ALLINVENTORYENTRIESLIST_RATE"]);
                        objALLINVENTORYENTRIES.AMOUNT = Convert.ToInt32(ConfigurationManager.AppSettings["ALLINVENTORYENTRIESLIST_AMOUNT"]);

                        decimal totqty = (Convert.ToDecimal(ds.Tables[1].Rows[i]["PackingNos"]) * Convert.ToDecimal(ds.Tables[1].Rows[i]["ProductQty"]));

                        string qty = (Convert.ToString(totqty) + " " + Convert.ToString(ds.Tables[1].Rows[i]["UOM"]).ToUpper() + " = " +
                            Convert.ToString(ds.Tables[1].Rows[i]["TotalKg"]) + " " + "KG."); // Convert.ToString(ds.Tables[1].Rows[i]["packingType"]));

                        objALLINVENTORYENTRIES.ACTUALQTY = qty;
                        objALLINVENTORYENTRIES.BILLEDQTY = qty;



                        TallyCls.BATCHALLOCATIONSLIST objBATCHALLOCATIONSLIST = new TallyCls.BATCHALLOCATIONSLIST();
                        objBATCHALLOCATIONSLIST.GODOWNNAME = Convert.ToString(ConfigurationManager.AppSettings["GODOWNNAME"]);
                        objBATCHALLOCATIONSLIST.BATCHNAME = Convert.ToString(ConfigurationManager.AppSettings["BATCHNAME"]);
                        objBATCHALLOCATIONSLIST.OrderDueDate = Convert.ToInt32(billdate);
                        objBATCHALLOCATIONSLIST.OrderDueDate = Convert.ToInt32(billdate);
                        objBATCHALLOCATIONSLIST.ACTUALQTY = qty;
                        objBATCHALLOCATIONSLIST.BILLEDQTY = qty;

                        #region UPVCHNOFPKGBTLIST
                        TallyCls.UPVCHNOFPKGBTLIST objUPVCHNOFPKGBTLIST = new TallyCls.UPVCHNOFPKGBTLIST();
                        objUPVCHNOFPKGBTLIST.DESC = Convert.ToString(ConfigurationManager.AppSettings["UPVCHNOFPKGBT_LIST_DESC"]);
                        objUPVCHNOFPKGBTLIST.ISLIST = Convert.ToString(ConfigurationManager.AppSettings["UPVCHNOFPKGBT_LIST_ISLIST"]);
                        objUPVCHNOFPKGBTLIST.TYPE = Convert.ToString(ConfigurationManager.AppSettings["UPVCHNOFPKGBT_LIST_TYPE"]);
                        objUPVCHNOFPKGBTLIST.INDEX = Convert.ToInt32(ConfigurationManager.AppSettings["UPVCHNOFPKGBT_LIST_INDEX"]);

                        TallyCls.UPVCHNOFPKGBT objUPVCHNOFPKGBT = new TallyCls.UPVCHNOFPKGBT();
                        objUPVCHNOFPKGBT.DESC = Convert.ToString(ConfigurationManager.AppSettings["UPVCHNOFPKGBT_DESC"]);
                        objUPVCHNOFPKGBT.Text = Convert.ToInt32(ds.Tables[1].Rows[i]["ProductQty"]);
                        objUPVCHNOFPKGBTLIST.UPVCHNOFPKGBT = objUPVCHNOFPKGBT;

                        objBATCHALLOCATIONSLIST.UPVCHNOFPKGBTLIST = objUPVCHNOFPKGBTLIST;
                        #endregion

                        objALLINVENTORYENTRIES.BATCHALLOCATIONSLIST = objBATCHALLOCATIONSLIST;




                        #region free item 

                        TallyCls.JRISBILLTRANSFRYNLIST objJRISBILLTRANSFRYNLIST = new TallyCls.JRISBILLTRANSFRYNLIST();
                        objJRISBILLTRANSFRYNLIST.DESC = Convert.ToString(ConfigurationManager.AppSettings["JRISBILLTRANSFRYNLIST_DESC"]);
                        objJRISBILLTRANSFRYNLIST.ISLIST = Convert.ToString(ConfigurationManager.AppSettings["JRISBILLTRANSFRYNLIST_ISLIST"]);
                        objJRISBILLTRANSFRYNLIST.TYPE = Convert.ToString(ConfigurationManager.AppSettings["JRISBILLTRANSFRYNLIST_TYPE"]);
                        objJRISBILLTRANSFRYNLIST.INDEX = Convert.ToInt32(ConfigurationManager.AppSettings["JRISBILLTRANSFRYNLIST_INDEX"]);

                        JRISBILLTRANSFRYN objJRISBILLTRANSFRYN = new JRISBILLTRANSFRYN();
                        objJRISBILLTRANSFRYN.DESC = Convert.ToString(ConfigurationManager.AppSettings["JRISBILLTRANSFRYNLIST_DESC"]);

                        if (Convert.ToString(ds.Tables[1].Rows[i]["OrderType"]) == "With Bill Free Scheme" ||
                            Convert.ToString(ds.Tables[1].Rows[i]["OrderType"]) == "Free Scheme")
                        {
                            objJRISBILLTRANSFRYN.Text = "Yes";
                        }
                        else
                        {
                            objJRISBILLTRANSFRYN.Text = "No";
                        }
                        objJRISBILLTRANSFRYNLIST.JRISBILLTRANSFRYN = objJRISBILLTRANSFRYN;

                        objALLINVENTORYENTRIES.JRISBILLTRANSFRYNLIST = objJRISBILLTRANSFRYNLIST;
                        #endregion


                        #region UPVCHNOFPKGEI


                        TallyCls.UPVCHNOFPKGEILIST objUPVCHNOFPKGEILIST = new TallyCls.UPVCHNOFPKGEILIST();
                        objUPVCHNOFPKGEILIST.DESC = Convert.ToString(ConfigurationManager.AppSettings["UPVCHNOFPKGEI_LIST_DESC"]);
                        objUPVCHNOFPKGEILIST.ISLIST = Convert.ToString(ConfigurationManager.AppSettings["UPVCHNOFPKGEI_LIST_ISLIST"]);
                        objUPVCHNOFPKGEILIST.TYPE = Convert.ToString(ConfigurationManager.AppSettings["UPVCHNOFPKGEI_LIST_TYPE"]);
                        objUPVCHNOFPKGEILIST.INDEX = Convert.ToInt32(ConfigurationManager.AppSettings["UPVCHNOFPKGEI_LIST_INDEX"]);

                        TallyCls.UPVCHNOFPKGEI objUPVCHNOFPKGEI = new TallyCls.UPVCHNOFPKGEI();
                        objUPVCHNOFPKGEI.DESC = Convert.ToString(ConfigurationManager.AppSettings["UPVCHNOFPKGEI_DESC"]);
                        objUPVCHNOFPKGEI.Text = Convert.ToInt32(ds.Tables[1].Rows[i]["ProductQty"]);
                        objUPVCHNOFPKGEILIST.UPVCHNOFPKGEI = objUPVCHNOFPKGEI;

                        objALLINVENTORYENTRIES.UPVCHNOFPKGEILIST = objUPVCHNOFPKGEILIST;


                        #endregion


                        objALLINVENTORYENTRIESLIST.Add(objALLINVENTORYENTRIES);
                    }
                }

                objVOUCHER.ALLINVENTORYENTRIESLIST = objALLINVENTORYENTRIESLIST;






                #endregion


                #region LEDGERENTRIESLIST
                TallyCls.LEDGERENTRIESLIST objLEDGERENTRIESLIST = new TallyCls.LEDGERENTRIESLIST();
                TallyCls.OLDAUDITENTRYIDSLIST objLEDGEROLDAUDITENTRYIDSLIST = new TallyCls.OLDAUDITENTRYIDSLIST();

                objLEDGEROLDAUDITENTRYIDSLIST.TYPE = Convert.ToString(ConfigurationManager.AppSettings["OLDAUDITENTRYIDSLISTLEDGERENTRIESLIST"]);
                objLEDGEROLDAUDITENTRYIDSLIST.OLDAUDITENTRYIDS = Convert.ToInt32(ConfigurationManager.AppSettings["OLDAUDITENTRYIDSLEDGERENTRIESLIST"]);
                objLEDGERENTRIESLIST.OLDAUDITENTRYIDSLIST = objLEDGEROLDAUDITENTRYIDSLIST;

                objLEDGERENTRIESLIST.LEDGERNAME = PARTYNAME;
                objLEDGERENTRIESLIST.GSTCLASS = Convert.ToString(ConfigurationManager.AppSettings["GSTCLASS"]);
                objLEDGERENTRIESLIST.ISDEEMEDPOSITIVE = Convert.ToString(ConfigurationManager.AppSettings["ISDEEMEDPOSITIVELEDGERENTRIESLIST"]);
                objLEDGERENTRIESLIST.LEDGERFROMITEM = Convert.ToString(ConfigurationManager.AppSettings["LEDGERFROMITEM"]);
                objLEDGERENTRIESLIST.REMOVEZEROENTRIES = Convert.ToString(ConfigurationManager.AppSettings["REMOVEZEROENTRIES"]);
                objLEDGERENTRIESLIST.ISPARTYLEDGER = Convert.ToString(ConfigurationManager.AppSettings["ISPARTYLEDGER"]);
                objLEDGERENTRIESLIST.ISLASTDEEMEDPOSITIVE = Convert.ToString(ConfigurationManager.AppSettings["ISLASTDEEMEDPOSITIVELEDGERENTRIESLIST"]);
                objLEDGERENTRIESLIST.ISCAPVATTAXALTERED = Convert.ToString(ConfigurationManager.AppSettings["ISCAPVATTAXALTERED"]);
                objLEDGERENTRIESLIST.ISCAPVATNOTCLAIMED = Convert.ToString(ConfigurationManager.AppSettings["ISCAPVATNOTCLAIMED"]);
                objLEDGERENTRIESLIST.AMOUNT = Convert.ToInt32(ConfigurationManager.AppSettings["LEDGERENTRIESLIST_AMOUNT"]);

                objVOUCHER.LEDGERENTRIESLIST = objLEDGERENTRIESLIST;
                #endregion LEDGERENTRIESLIST

                objTALLYMESSAGE.VOUCHER = objVOUCHER;
                objREQUESTDATA.TALLYMESSAGE = objTALLYMESSAGE;
                //#endregion -- REQUESTDATA

                objIMPORTDATA.REQUESTDATA = objREQUESTDATA;
                objBODY.IMPORTDATA = objIMPORTDATA;

                objENVELOPE.BODY = objBODY;
                #endregion



                #region xml data insert in table 
                string xml = GetXMLFromObject(objENVELOPE);

                BA_XmlData ObjBA_XmlData = new BA_XmlData();
                ObjBA_XmlData.INSERT_XmlData(xml, OrderID);
                #endregion

                return xml;
            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("xmldataread Error == " + ex.ToString());
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "xmldataread");
                return "";
            }
        }

        public string GetXMLFromObject(object o)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;

            try
            {
                XmlSerializer s = new XmlSerializer(typeof(ENVELOPE));
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                tw = new XmlTextWriter(sw);

                s.Serialize(tw, o, ns);
            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("GetXMLFromObject error  " + ex.ToString());
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "GetXMLFromObject");
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }

            }
            return sw.ToString();
        }


        public bool SendTallyRequest(string _requestString)
        {
            ResponseModel _responseModel = new ResponseModel();
            try
            {
                string _tallyHost = Convert.ToString(ConfigurationManager.AppSettings["HostUrl"].ToString());

                HttpWebRequest _webRequest = (HttpWebRequest)WebRequest.Create(_tallyHost);
                _webRequest.Method = "POST";
                _webRequest.ContentLength = (long)_requestString.Length;
                _webRequest.ContentType = "application/x-www-form-urlencoded";

                StreamWriter _sWriter = new StreamWriter(_webRequest.GetRequestStream());
                _sWriter.Write(_requestString);
                _sWriter.Close();

                HttpWebResponse _webResponse = (HttpWebResponse)_webRequest.GetResponse();
                Stream _responseStream = _webResponse.GetResponseStream();

                StreamReader _sReader = new StreamReader(_responseStream, Encoding.UTF8);

                string _responseString = _sReader.ReadToEnd();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_responseString);
                doc.Save(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.xml");

                DataSet ds = new DataSet();
                ds.ReadXml(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.xml");

                _responseModel.dsResponse = ds;
                _responseModel.StrResponse = _responseString;
                _webResponse.Close();
                _sReader.Close();

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {

                    DataColumnCollection columns = ds.Tables[0].Columns;
                    if (columns.Contains("CREATED"))
                    {
                        if (Convert.ToString(ds.Tables[0].Rows[0]["CREATED"]) == "1")
                        {
                            return true;
                        }
                        else
                        {
                            BA_XmlData ObjBA_XmlData = new BA_XmlData();
                            ObjBA_XmlData.INSERT_XmlData(_requestString, 0);
                            sendmailXMLError(ds);
                            return false;
                        }
                    }
                    else
                    {
                        BA_XmlData ObjBA_XmlData = new BA_XmlData();
                        ObjBA_XmlData.INSERT_XmlData(_requestString, 0);
                        sendmailXMLError(ds);
                        return false;
                    }
                }
                else
                {
                    BA_XmlData ObjBA_XmlData = new BA_XmlData();
                    ObjBA_XmlData.INSERT_XmlData(_requestString, 0);
                    sendmailXMLError(ds);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("SendTallyRequest error = " + ex.ToString());
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "SendTallyRequest");
                return false;
            }
        }

        public DataSet GetTallyData()
        {
            try
            {
                var rmsConnectionString = Connection.GetSConnectionString();
                ConnectionString = rmsConnectionString;
                DataSet odata = SqlHelper.ExecuteDataset(rmsConnectionString, CommandType.StoredProcedure, "Proc_GetImportTallyData");
                return odata;
            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("GetTallyData error = " + ex.ToString());
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "GetTallyData");
                return null;
            }
        }

        public DataSet PostTallyData(int orderid)
        {
            try
            {
                var rmsConnectionString = Connection.GetSConnectionString();
                ConnectionString = rmsConnectionString;
                DataSet odata = SqlHelper.ExecuteDataset(rmsConnectionString, CommandType.StoredProcedure, "GetOrderPostTally",
                    new SqlParameter("@orderid", orderid));
                return odata;
            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("PostTallyData error = " + ex.ToString());
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "PostTallyData");
                return null;
            }
        }

        public long UpdateImportTallyData(int orderid)
        {
            try
            {
                return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, "UpdateOrderafterTallyImport",
                                 new SqlParameter("@orderid", orderid));
            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("UpdateImportTallyData error = " + ex.ToString());
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "UpdateImportTallyData");
                return 0;
            }
        }


      
        public void sendmailXMLError(DataSet ds)
        {
            string emailbody = "";
            try
            {
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    emailbody = ConfigurationManager.AppSettings["Message"].ToString() + ": Data not insert in tally";


                    foreach (DataColumn col in ds.Tables[0].Columns)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            emailbody = emailbody + "/n" + row[col.ColumnName] + Convert.ToString(row.ItemArray);
                        }
                    }
                }
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(null, emailbody);
            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("sendmailXMLError error = " + ex.ToString());
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "sendmailXMLError");
            }
            
           
        }


        public void sendmail(DataSet ds)
        {
            string emailbody = "";
            try
            {
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    emailbody = ConfigurationManager.AppSettings["Message"].ToString() + ": Data not insert in tally";
                    emailbody = emailbody + "/n" + Convert.ToString(ds.Tables[0].Rows[0]["OrderSrNo"]) + ", Order Date; " + Convert.ToDateTime(ds.Tables[0].Rows[0]["OrderDate"]).ToString();
                    emailbody = emailbody + "/n" + "Party name :" + Convert.ToString(ds.Tables[0].Rows[0]["DealerName"]) + " pincode : " + Convert.ToString(ds.Tables[0].Rows[0]["Pincode"]);
                    emailbody = emailbody + "/n" + "party state:" + Convert.ToString(ds.Tables[0].Rows[0]["state_name"]);
                }

                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(null, emailbody);
            }
            catch (Exception ex)
            {
                ServiceLog.WriteErrorLog("sendmailXMLError error = " + ex.ToString());
                BA_ErrorLog ObjError = new BA_ErrorLog();
                ObjError.INSERT_ErrorLog(ex, "sendmailXMLError");
            }
        }
    }
}
