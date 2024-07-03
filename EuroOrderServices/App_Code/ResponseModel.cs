using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuroOrderServices.App_Code
{
    public class ResponseModel
    {
        public DataSet dsResponse;

        public string StrResponse;
        public string Company;
        public string strVchType;
        public string strVchNumber;
        public string strDate;
        public string strNarration;
        public string strVoucherEntryName1;
        public string strISDEEMEDPOSITIVE1;
        public string strAmount1;
        public string strVoucherEntryName2;
        public string strISDEEMEDPOSITIVE2;
        public string strAmount2;

        public int intMasterId;
    }
}
