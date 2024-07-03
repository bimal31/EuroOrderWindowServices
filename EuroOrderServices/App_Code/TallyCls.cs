using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EuroOrderServices.App_Code
{
    public partial class TallyCls
    {
        // using this link converet xml to cls
        //https://json2csharp.com/code-converters/xml-to-csharp
        // using System.Xml.Serialization;
        // XmlSerializer serializer = new XmlSerializer(typeof(ENVELOPE));
        // using (StringReader reader = new StringReader(xml))
        // {
        //    var test = (ENVELOPE)serializer.Deserialize(reader);
        // }

        
        [XmlRoot(ElementName = "ENVELOPE")]
		public class ENVELOPE
		{
            [XmlNamespaceDeclarations]
            public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();

            [XmlElement(ElementName = "HEADER")]
			public HEADER HEADER { get; set; }

			[XmlElement(ElementName = "BODY")]
			public BODY BODY { get; set; }

            public ENVELOPE()
            {
                xmlns.Add("UDF", "TallyNamespace");
            }
        }






		[XmlRoot(ElementName = "HEADER")]
		public class HEADER
		{

			[XmlElement(ElementName = "TALLYREQUEST")]
			public string TALLYREQUEST { get; set; }
		}

		[XmlRoot(ElementName = "BODY")]
		public class BODY
		{

			[XmlElement(ElementName = "IMPORTDATA")]
			public IMPORTDATA IMPORTDATA { get; set; }
		}

		[XmlRoot(ElementName = "IMPORTDATA")]
		public class IMPORTDATA
		{

			[XmlElement(ElementName = "REQUESTDESC")]
			public REQUESTDESC REQUESTDESC { get; set; }

			[XmlElement(ElementName = "REQUESTDATA")]
			public REQUESTDATA REQUESTDATA { get; set; }
		}

		[XmlRoot(ElementName = "REQUESTDESC")]
		public class REQUESTDESC
		{

			[XmlElement(ElementName = "REPORTNAME")]
			public string REPORTNAME { get; set; }

			[XmlElement(ElementName = "STATICVARIABLES")]
			public STATICVARIABLES STATICVARIABLES { get; set; }
		}

		[XmlRoot(ElementName = "STATICVARIABLES")]
		public class STATICVARIABLES
		{

			[XmlElement(ElementName = "SVCURRENTCOMPANY")]
			public string SVCURRENTCOMPANY { get; set; }
		}


		[XmlRoot(ElementName = "REQUESTDATA")]
		public class REQUESTDATA
		{

			[XmlElement(ElementName = "TALLYMESSAGE")]
			public TALLYMESSAGE TALLYMESSAGE { get; set; }
		}

		[XmlRoot(ElementName = "TALLYMESSAGE")]
		public class TALLYMESSAGE
		{

         

            [XmlElement(ElementName = "VOUCHER")]
            public VOUCHER VOUCHER { get; set; }

            [XmlAttribute(AttributeName = "UDF")]
			public string UDF { get; set; }

            
			[XmlText]
			public string Text { get; set; }

          
        }

		[XmlRoot(ElementName = "VOUCHER")]
		public class VOUCHER
		{

			[XmlElement(ElementName = "ADDRESS.LIST")]
			public ADDRESSLIST ADDRESSLIST { get; set; }

            [XmlElement(ElementName = "DISPATCHFROMADDRESS.LIST")]
            public DISPATCHFROMADDRESSLIST DISPATCHFROMADDRESSLIST { get; set; }

            [XmlElement(ElementName = "BASICBUYERADDRESS.LIST")]
            public BASICBUYERADDRESSLIST BASICBUYERADDRESSLIST { get; set; }

            [XmlElement(ElementName = "OLDAUDITENTRYIDS.LIST")]
            public OLDAUDITENTRYIDSLIST OLDAUDITENTRYIDSLIST { get; set; }

            [XmlElement(ElementName = "DATE")]
			public int DATE { get; set; }

			[XmlElement(ElementName = "IRNACKDATE")]
			public int IRNACKDATE { get; set; }

			[XmlElement(ElementName = "GUID")]
			public string GUID { get; set; }

			[XmlElement(ElementName = "GSTREGISTRATIONTYPE")]
			public string GSTREGISTRATIONTYPE { get; set; }

			[XmlElement(ElementName = "VATDEALERTYPE")]
			public string VATDEALERTYPE { get; set; }

			[XmlElement(ElementName = "STATENAME")]
			public string STATENAME { get; set; }

			[XmlElement(ElementName = "COUNTRYOFRESIDENCE")]
			public string COUNTRYOFRESIDENCE { get; set; }

			[XmlElement(ElementName = "PARTYGSTIN")]
			public string PARTYGSTIN { get; set; }

			[XmlElement(ElementName = "PLACEOFSUPPLY")]
			public string PLACEOFSUPPLY { get; set; }

			[XmlElement(ElementName = "CLASSNAME")]
			public string CLASSNAME { get; set; }

			[XmlElement(ElementName = "PARTYNAME")]
			public string PARTYNAME { get; set; }

			[XmlElement(ElementName = "PARTYLEDGERNAME")]
			public string PARTYLEDGERNAME { get; set; }

			[XmlElement(ElementName = "VOUCHERTYPENAME")]
			public string VOUCHERTYPENAME { get; set; }

			[XmlElement(ElementName = "PARTYMAILINGNAME")]
			public string PARTYMAILINGNAME { get; set; }

			[XmlElement(ElementName = "PARTYPINCODE")]
			public int PARTYPINCODE { get; set; }

			[XmlElement(ElementName = "CONSIGNEEGSTIN")]
			public string CONSIGNEEGSTIN { get; set; }

			[XmlElement(ElementName = "CONSIGNEEMAILINGNAME")]
			public string CONSIGNEEMAILINGNAME { get; set; }

			[XmlElement(ElementName = "CONSIGNEEPINCODE")]
			public int CONSIGNEEPINCODE { get; set; }

			[XmlElement(ElementName = "CONSIGNEESTATENAME")]
			public string CONSIGNEESTATENAME { get; set; }

			[XmlElement(ElementName = "VOUCHERNUMBER")]
			public string VOUCHERNUMBER { get; set; }

			[XmlElement(ElementName = "Reference")]
			public string Reference { get; set; }

			[XmlElement(ElementName = "BASICBASEPARTYNAME")]
			public string BASICBASEPARTYNAME { get; set; }

			[XmlElement(ElementName = "CSTFORMISSUETYPE")]
			public string CSTFORMISSUETYPE { get; set; }

			[XmlElement(ElementName = "CSTFORMRECVTYPE")]
			public string CSTFORMRECVTYPE { get; set; }

			[XmlElement(ElementName = "FBTPAYMENTTYPE")]
			public string FBTPAYMENTTYPE { get; set; }

			[XmlElement(ElementName = "PERSISTEDVIEW")]
			public string PERSISTEDVIEW { get; set; }

			[XmlElement(ElementName = "BASICBUYERNAME")]
			public string BASICBUYERNAME { get; set; }

			[XmlElement(ElementName = "CONSIGNEECOUNTRYNAME")]
			public string CONSIGNEECOUNTRYNAME { get; set; }

			[XmlElement(ElementName = "VCHGSTCLASS")]
			public string VCHGSTCLASS { get; set; }

			[XmlElement(ElementName = "VCHENTRYMODE")]
			public string VCHENTRYMODE { get; set; }

			[XmlElement(ElementName = "VOUCHERTYPEORIGNAME")]
			public string VOUCHERTYPEORIGNAME { get; set; }

			[XmlElement(ElementName = "DIFFACTUALQTY")]
			public string DIFFACTUALQTY { get; set; }

			[XmlElement(ElementName = "ISMSTFROMSYNC")]
			public string ISMSTFROMSYNC { get; set; }

			[XmlElement(ElementName = "ASORIGINAL")]
			public string ASORIGINAL { get; set; }

			[XmlElement(ElementName = "AUDITED")]
			public string AUDITED { get; set; }

			[XmlElement(ElementName = "FORJOBCOSTING")]
			public string FORJOBCOSTING { get; set; }

			[XmlElement(ElementName = "ISOPTIONAL")]
			public string ISOPTIONAL { get; set; }

			[XmlElement(ElementName = "EFFECTIVEDATE")]
			public int EFFECTIVEDATE { get; set; }

			[XmlElement(ElementName = "ISINVOICE")]
			public string ISINVOICE { get; set; }

            [XmlElement(ElementName = "ALLINVENTORYENTRIES.LIST")]
            public List<ALLINVENTORYENTRIESLIST> ALLINVENTORYENTRIESLIST { get; set; }

            [XmlElement(ElementName = "LEDGERENTRIES.LIST")]
            public LEDGERENTRIESLIST LEDGERENTRIESLIST { get; set; }

            [XmlAttribute(AttributeName = "REMOTEID")]
			public string REMOTEID { get; set; }

			[XmlAttribute(AttributeName = "VCHKEY")]
			public string VCHKEY { get; set; }

			[XmlAttribute(AttributeName = "SENDERID")]
			public string SENDERID { get; set; }

			[XmlAttribute(AttributeName = "VCHTYPE")]
			public string VCHTYPE { get; set; }

			[XmlAttribute(AttributeName = "ACTION")]
			public string ACTION { get; set; }

			[XmlAttribute(AttributeName = "OBJVIEW")]
			public string OBJVIEW { get; set; }

			[XmlText]
			public string Text { get; set; }
		}
		[XmlRoot(ElementName = "ADDRESS.LIST")]
		public class ADDRESSLIST
		{

			[XmlElement(ElementName = "ADDRESS")]
			public List<string> ADDRESS { get; set; }

			[XmlAttribute(AttributeName = "TYPE")]
			//public string TYPE { get; set; }

			[XmlText]
			public string Text { get; set; }
		}

		[XmlRoot(ElementName = "DISPATCHFROMADDRESS.LIST")]
		public class DISPATCHFROMADDRESSLIST
		{

			[XmlElement(ElementName = "DISPATCHFROMADDRESS")]
			public List<string> DISPATCHFROMADDRESS { get; set; }

			[XmlAttribute(AttributeName = "TYPE")]
			public string TYPE { get; set; }

			[XmlText]
			public string Text { get; set; }
		}

		[XmlRoot(ElementName = "BASICBUYERADDRESS.LIST")]
		public class BASICBUYERADDRESSLIST
		{

			[XmlElement(ElementName = "BASICBUYERADDRESS")]
			public List<string> BASICBUYERADDRESS { get; set; }

			[XmlAttribute(AttributeName = "TYPE")]
			public string TYPE { get; set; }

			[XmlText]
			public string Text { get; set; }
		}

		[XmlRoot(ElementName = "OLDAUDITENTRYIDS.LIST")]
		public class OLDAUDITENTRYIDSLIST
		{

			[XmlElement(ElementName = "OLDAUDITENTRYIDS")]
			public int OLDAUDITENTRYIDS { get; set; }

			[XmlAttribute(AttributeName = "TYPE")]
			public string TYPE { get; set; }

			//[XmlText]
			//public int Text { get; set; }
		}

        [XmlRoot(ElementName = "BATCHALLOCATIONS.LIST")]
        public class BATCHALLOCATIONSLIST
        {

            [XmlElement(ElementName = "GODOWNNAME")]
            public string GODOWNNAME { get; set; }

            [XmlElement(ElementName = "BATCHNAME")]
            public string BATCHNAME { get; set; }

            [XmlElement(ElementName = "OrderDueDate")]
            public int OrderDueDate { get; set; }

            [XmlElement(ElementName = "AMOUNT")]
            public int AMOUNT { get; set; }

            [XmlElement(ElementName = "ACTUALQTY")]
            public string ACTUALQTY { get; set; }

            [XmlElement(ElementName = "BILLEDQTY")]
            public string BILLEDQTY { get; set; }


            [XmlElement(ElementName = "UPVCHNOFPKGBT.LIST", Namespace = "TallyNamespace")]
            public UPVCHNOFPKGBTLIST UPVCHNOFPKGBTLIST { get; set; }
           
        }


        #region bactchlist qty cal

        [XmlRoot(ElementName = "UPVCHNOFPKGBT.LIST")]
        public class UPVCHNOFPKGBTLIST
        {

            [XmlElement(ElementName = "UPVCHNOFPKGBT", Namespace = "TallyNamespace")]
            public UPVCHNOFPKGBT UPVCHNOFPKGBT { get; set; }

            [XmlAttribute(AttributeName = "DESC")]
            public string DESC { get; set; }

            [XmlAttribute(AttributeName = "ISLIST")]
            public string ISLIST { get; set; }

            [XmlAttribute(AttributeName = "TYPE")]
            public string TYPE { get; set; }

            [XmlAttribute(AttributeName = "INDEX")]
            public int INDEX { get; set; }

        }

        [XmlRoot(ElementName = "UPVCHNOFPKGBT")]
        public class UPVCHNOFPKGBT
        {

            [XmlAttribute(AttributeName = "DESC")]
            public string DESC { get; set; }

            [XmlText]
            public int Text { get; set; }
        }
        #endregion

		[XmlRoot(ElementName = "ALLINVENTORYENTRIES.LIST")]
		public class ALLINVENTORYENTRIESLIST
		{

			[XmlElement(ElementName = "STOCKITEMNAME")]
			public string STOCKITEMNAME { get; set; }

			[XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
			public string ISDEEMEDPOSITIVE { get; set; }

			[XmlElement(ElementName = "ISLASTDEEMEDPOSITIVE")]
			public string ISLASTDEEMEDPOSITIVE { get; set; }

			[XmlElement(ElementName = "RATE")]
			public int RATE { get; set; }

			[XmlElement(ElementName = "AMOUNT")]
			public int AMOUNT { get; set; }

			[XmlElement(ElementName = "ACTUALQTY")]
			public string ACTUALQTY { get; set; }

			[XmlElement(ElementName = "BILLEDQTY")]
			public string BILLEDQTY { get; set; }

			[XmlElement(ElementName = "BATCHALLOCATIONS.LIST")]
			public BATCHALLOCATIONSLIST BATCHALLOCATIONSLIST { get; set; }



			#region add fre item 
			[XmlElement(ElementName = "JRSIVchIsDiscItmYN.LIST")]
			public JRISBILLTRANSFRYNLIST JRISBILLTRANSFRYNLIST { get; set; }
            #endregion

            [XmlElement(ElementName = "UPVCHNOFPKGEI.LIST", Namespace = "TallyNamespace")]
            public UPVCHNOFPKGEILIST UPVCHNOFPKGEILIST { get; set; }
        }


        [XmlRoot(ElementName = "UPVCHNOFPKGEI.LIST")]
        public class UPVCHNOFPKGEILIST
        {
            [XmlElement(ElementName = "UPVCHNOFPKGEI", Namespace = "TallyNamespace")]
            public UPVCHNOFPKGEI UPVCHNOFPKGEI { get; set; }

            [XmlAttribute(AttributeName = "DESC")]
            public string DESC { get; set; }

            [XmlAttribute(AttributeName = "ISLIST")]
            public string ISLIST { get; set; }

            [XmlAttribute(AttributeName = "TYPE")]
            public string TYPE { get; set; }

            [XmlAttribute(AttributeName = "INDEX")]
            public int INDEX { get; set; }

        }

        [XmlRoot(ElementName = "UPVCHNOFPKGEI")]
        public class UPVCHNOFPKGEI
        {

            [XmlAttribute(AttributeName = "DESC")]
            public string DESC { get; set; }

            [XmlText]
            public int Text { get; set; }
        }


        [XmlRoot(ElementName = "JRISBILLTRANSFRYN")]
		public class JRISBILLTRANSFRYN
		{

			[XmlAttribute(AttributeName = "DESC")]
			public string DESC { get; set; }

			[XmlText]
			public string Text { get; set; }
		}

		[XmlRoot(ElementName = "JRSIVchIsDiscItmYN.LIST")]
		public class JRISBILLTRANSFRYNLIST
		{

			[XmlElement(ElementName = "JRSIVchIsDiscItmYN")]
			public JRISBILLTRANSFRYN JRISBILLTRANSFRYN { get; set; }

			[XmlAttribute(AttributeName = "DESC")]
			public string DESC { get; set; }

			[XmlAttribute(AttributeName = "ISLIST")]
			public string ISLIST { get; set; }

			[XmlAttribute(AttributeName = "TYPE")]
			public string TYPE { get; set; }

			[XmlAttribute(AttributeName = "INDEX")]
			public int INDEX { get; set; }

			[XmlText]
			public string Text { get; set; }
		}


		[XmlRoot(ElementName = "LEDGERENTRIES.LIST")]
		public class LEDGERENTRIESLIST
		{

			[XmlElement(ElementName = "OLDAUDITENTRYIDS.LIST")]
			public OLDAUDITENTRYIDSLIST OLDAUDITENTRYIDSLIST { get; set; }

			[XmlElement(ElementName = "LEDGERNAME")]
			public string LEDGERNAME { get; set; }

			[XmlElement(ElementName = "GSTCLASS")]
			public string GSTCLASS { get; set; }

			[XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
			public string ISDEEMEDPOSITIVE { get; set; }

			[XmlElement(ElementName = "LEDGERFROMITEM")]
			public string LEDGERFROMITEM { get; set; }

			[XmlElement(ElementName = "REMOVEZEROENTRIES")]
			public string REMOVEZEROENTRIES { get; set; }

			[XmlElement(ElementName = "ISPARTYLEDGER")]
			public string ISPARTYLEDGER { get; set; }

			[XmlElement(ElementName = "ISLASTDEEMEDPOSITIVE")]
			public string ISLASTDEEMEDPOSITIVE { get; set; }

			[XmlElement(ElementName = "ISCAPVATTAXALTERED")]
			public string ISCAPVATTAXALTERED { get; set; }

			[XmlElement(ElementName = "ISCAPVATNOTCLAIMED")]
			public string ISCAPVATNOTCLAIMED { get; set; }

			[XmlElement(ElementName = "AMOUNT")]
			public int AMOUNT { get; set; }
		}

		

		

		
	
		

	

	}
}
