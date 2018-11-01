using EdiFabric.Templates.X12;
using EdiFabric.Core.Model.Edi.X12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace EDI.Fabric.Libraries.Writers.Types
{
    class X12850
    {
        private static Dictionary<int, Dictionary<string, string>> data = new Dictionary<int, Dictionary<string, string>>();

        public X12850(string theFile, string _portID = null, string status = "Send")
        {
            // create EDI
            switch(status)
            {
                case "Send":
                    // @todo - pull dates from ... config file maybe ???
                    BuildEDI(_portID, DateTime.Now.AddDays(-7).ToString("yyyyMMdd"), DateTime.Now.ToString("yyyyMMdd"));
                    break;
                case "Respond":
                    BuildResponse();
                    break;
                default:
                    throw new Exception("The requested status does not exist");
            }
            // write string

            // update database
        }
        static void getData(string portID, string startDate, string endDate)
        {
            Libraries.Data.Get g = new Libraries.Data.Get(EDI.Fabric.Program.currentDBconn);
            data = g.GetTransactions(portID, startDate, endDate);
        }
        static TS850 BuildEDI(string portID, string startDate, string endDate) {
            // In 99% of the cases this will be about 2kb
            // overall maybe 10 - 20 transactions
            // 
            // 

            getData(portID, startDate, endDate);

            var dtest = data;

            bool firstRow = true;

            var result = new TS850();

            int iLength = 0;
            double dAmt = 0;
            foreach (var d in data)
            {
                if (firstRow == true)
                {
                    var test = d;
                    result.ST = new ST();
                    result.ST.TransactionSetIdentifierCode_01 = "850";

                    // ask if this is created at reuntime or pulled
                    //result.ST.TransactionSetControlNumber_02 = d.Value["controlNumber"].PadLeft(9, '0');

                    result.BEG = new BEG();
                    result.BEG.TransactionSetPurposeCode_01 = "00";
                    result.BEG.PurchaseOrderTypeCode_02 = "SA";
                    result.BEG.PurchaseOrderNumber_03 = d.Value["POID"];
                    //result.BEG.Date_05 = DateTime.TryParseExact("20181001","yyyyMMdd").ToString(); //DateTime.Now.ToString("yyyyMMdd");
                   
                    //result.BEG.ContractNumber_06 = d.Value["ContractNumber"];
                    firstRow = false;

                    result.N1Loop = new List<Loop_N1_850>();
                    var n1Loop = new Loop_N1_850();
                    n1Loop.N1 = new N1();
                    n1Loop.N1.EntityIdentifierCode_01 = "ST";
                    n1Loop.N1.Name_02 = d.Value["ShiptoName"]; // Verify
                    n1Loop.N1.IdentificationCodeQualifier_03 = d.Value["ShiptoIDQ"];
                    n1Loop.N1.IdentificationCode_04 = "123456789"; //d.Value["IC"]; // Ask 
                    /*
                    n1Loop.N2 = new List<N2>();
                    var n2 = new N2();
                    n2.Name_01 = 
                    */
                    n1Loop.N3 = new List<N3>();
                    var n3 = new N3();
                    n3.AddressInformation_01 = d.Value["ShiptoAddress"];
                    n1Loop.N3.Add(n3);

                    n1Loop.N4 = new List<N4>();
                    var n4 = new N4();
                    n4.CityName_01 = d.Value["ShiptoCity"];
                    n4.StateorProvinceCode_02 = d.Value["ShiptoState"];
                    n4.PostalCode_03 = d.Value["ShiptoZip"];
                    n1Loop.N4.Add(n4);

                    result.N1Loop.Add(n1Loop); 
                } // End First Row Loop

                iLength++;
                dAmt += (float.Parse(d.Value["ItemPrice"],CultureInfo.InvariantCulture) * int.Parse(d.Value["ItemQty"]));

            } // End data loop

            //  Begin CTT Loop   
            result.CTTLoop = new Loop_CTT_850();

            //  Indicates that the purchase order contains 1 line item.
            result.CTTLoop.CTT = new CTT();
            result.CTTLoop.CTT.NumberofLineItems_01 = iLength.ToString();

            //  Indicates that the total amount of the purchase order is $900.
            result.CTTLoop.AMT = new AMT();
            result.CTTLoop.AMT.AmountQualifierCode_01 = "";
            result.CTTLoop.AMT.MonetaryAmount_02 = Math.Round(dAmt,2).ToString();
            // End CT Loop

            return result;
        }
        static TS850 BuildResponse() {
            var result = new TS850();


            return result;
        }
    }
}
