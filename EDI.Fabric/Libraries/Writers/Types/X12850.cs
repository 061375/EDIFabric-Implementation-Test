using EdiFabric.Templates.X12;
using System;
using System.Collections.Generic;
using System.Globalization;
using EdiFabric.Framework.Writers;
using EdiFabric.Core.Model.Edi.ErrorContexts;
using System.Diagnostics;
using EdiFabric.Core.Model.Edi.X12;

namespace EDI.Fabric.Libraries.Writers.Types
{
    class X12850
    {
        private static Dictionary<int, Dictionary<string, string>> data = new Dictionary<int, Dictionary<string, string>>();

        // Sub Classes
        private static readonly SubClass.Acme Acme = new SubClass.Acme();

        public static string GetHDASN = null;
        public static string ASNID = null; // @todo get and update this value
        public static string ASNPurpose = null;
        public static string SSCC = null;
        public static string PONUM = null;
        public static string PortID = null;

        public X12850(string _PortID, string _PONUM, string _SSCC)
        {
            PortID = _PortID;
            PONUM = _PONUM;
            SSCC = _SSCC;

            // based on the customer create a connection to the database and get the transaction data
            switch (PortID)
            {
                case "ACME":
                    Libraries.Data.Customer.ACMEASN dAcme = new Libraries.Data.Customer.ACMEASN(PortID, PONUM, "856");
                    data = dAcme.GetData;
                    WriteSingleInvoiceToFile(BuildEDI(dAcme));
                    break;
                default:
                    Libraries.Data.Customer.General dGeneral = new Libraries.Data.Customer.General(PortID, PONUM, "856");
                    data = dGeneral.GetData;
                    WriteSingleInvoiceToFile(BuildEDI(dGeneral));
                    break;
            }
        }
        static TS850 BuildEDI(Interfaces.GetASNDataInterface _data) {
            // In 99% of the cases this will be about 2kb
            // overall maybe 10 - 20 transactions
            // 
            // 
            int iLength = 0;
            double dAmt = 0;
            bool firstRow = true;
            var result = new TS850();
                result.PO1Loop = new List<Loop_PO1_850>();
            var PO1loop = new Loop_PO1_850();
            

            foreach (var d in data)
            {
                // this allows me to pass d to the method
                Dictionary<string, string> dd = new Dictionary<string, string>(d.Value);
                // deal with the tature of the data returned
                if (firstRow == true)
                {
                    firstRow = false;
                    BuildST(dd,ref result);
                    BuildBEG(dd,ref result);
                    var n1Loop = BuildN1(dd,ref result);
                    BuildN3(dd, n1Loop, ref result);
                    BuildN4(dd, n1Loop, ref result);
                    result.N1Loop.Add(n1Loop); 
                } // End First Row Loop

                //  Begin Looped Content
                //  Indicates Baseline item 1 is a request to purchase 25 units, 
                //  with a price of $36.00 each, of manufacturer's part number XYZ-1234.
                PO1loop.PO1 = new PO1();
                PO1loop.PO1 = BuildPO1loop(dd, new PO1(), ref result);
                // Followed the template
                // This will get easier te more I read them
                PO1loop.PIDLoop = new List<Loop_PID_850>();
                    var loopPID = new Loop_PID_850();
                        loopPID.PID = new PID();
                            loopPID.PID.ItemDescriptionType_01 = ""; // ironically enough after all this there is no data fields that correspond
                            loopPID.PID.Description_05 = ""; // But it helps me learn
                PO1loop.PIDLoop.Add(loopPID);
                

                // Moved the ADD PO1loop HERE
                result.PO1Loop.Add(PO1loop);

                // Begin Incrementing Data
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
        static void BuildST(Dictionary<string, string> d, ref TS850 result)
        {
            result.ST = new ST();
            result.ST.TransactionSetIdentifierCode_01 = "850";
            // ask if this is created at reuntime or pulled
            //result.ST.TransactionSetControlNumber_02 = d["controlNumber"].PadLeft(9, '0');
        }
        static void BuildBEG(Dictionary<string, string> d,ref TS850 result)
        {
            result.BEG = new BEG();
            result.BEG.TransactionSetPurposeCode_01 = Constants.X12856BEG01;
            result.BEG.PurchaseOrderTypeCode_02 = Constants.X12856BEG02;
            result.BEG.PurchaseOrderNumber_03 = d["POID"];
            //result.BEG.Date_05 = DateTime.TryParseExact("20181001","yyyyMMdd").ToString(); //DateTime.Now.ToString("yyyyMMdd");
            //result.BEG.ContractNumber_06 = d["ContractNumber"];
        }
        static Loop_N1_850 BuildN1(Dictionary<string, string> d,ref TS850 result)
        {
            result.N1Loop = new List<Loop_N1_850>();
            var n1Loop = new Loop_N1_850();
            n1Loop.N1 = new N1();
            n1Loop.N1.EntityIdentifierCode_01 = Constants.X12850N101;
            n1Loop.N1.Name_02 = d["ShiptoName"]; // Verify
            n1Loop.N1.IdentificationCodeQualifier_03 = d["ShiptoIDQ"];
            n1Loop.N1.IdentificationCode_04 = d["ShiptoID"]; 
            return n1Loop;
        }
        static void BuildN2(Dictionary<string, string> d,Loop_N1_850 n1Loop, ref TS850 result)
        {
            
            n1Loop.N2 = new List<N2>();
            var n2 = new N2();
            n2.Name_01 = Program.ShipFromCompanyName;
            
        }
        static void BuildN3(Dictionary<string, string> d, Loop_N1_850 n1Loop, ref TS850 result)
        {
            n1Loop.N3 = new List<N3>();
            var n3 = new N3();
            n3.AddressInformation_01 = d["ShiptoAddress"];
            n1Loop.N3.Add(n3);
        }
        static void BuildN4(Dictionary<string, string> d, Loop_N1_850 n1Loop, ref TS850 result)
        {
            n1Loop.N4 = new List<N4>();
            var n4 = new N4();
            n4.CityName_01 = d["ShiptoCity"];
            n4.StateorProvinceCode_02 = d["ShiptoState"];
            n4.PostalCode_03 = d["ShiptoZip"];
            n1Loop.N4.Add(n4);
        }
        static PO1 BuildPO1loop(Dictionary<string, string> d, PO1 PO1, ref TS850 result)
        {
            PO1.AssignedIdentification_01 = d["ItemSQ"]; // Ask
            PO1.QuantityOrdered_02 = d["ItemQty"];
            PO1.UnitorBasisforMeasurementCode_03 = d["ItemUnit"];
            PO1.UnitPrice_04 = float.Parse(d["ItemPrice"], CultureInfo.InvariantCulture).ToString();
            PO1.BasisofUnitPriceCode_05 = Constants.X12850PO101; 
            PO1.ProductServiceIDQualifier_06 = d["ItemIQ2"]; 
            PO1.ProductServiceID_07 = d["ItemID2"];
            return PO1;
        }
        /// <summary>
        /// Generate and write EDI document to a file
        /// </summary>
        static void WriteSingleInvoiceToFile(TS850 invoice)
        {

            //  2.  Validate it by skipping trailer validation
            MessageErrorContext errorContext;
            if (invoice.IsValid(out errorContext, true))
            {
                Debug.WriteLine("Message {0} with control number {1} is valid.", errorContext.Name,
                    errorContext.ControlNumber);

                //  3.  Write directly to a file
                using (var writer = new X12Writer(@"C:\Temp\Output.txt", false))
                {
                    writer.Write(Helpers.Helpers.BuildIsa(ASNID));
                    writer.Write(Helpers.Helpers.BuildGs(ASNID));
                    writer.Write(invoice);
                }

                Debug.WriteLine("Written to file.");
            }
            else
            {
                //  The invoice is invalid
                Debug.WriteLine("The invoice is invalid");
            }
        }
    }
}