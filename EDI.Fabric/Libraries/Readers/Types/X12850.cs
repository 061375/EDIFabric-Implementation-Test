using Edi.Fabric.Templates;
using EdiFabric.Core.Model.Edi;
using EdiFabric.Framework;
using EdiFabric.Framework.Readers;
using EdiFabric.Sdk.Helpers;
using EdiFabric.Templates.X12;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
/** 
 * 
 * EDI.Fabric X12850 Extract an insert class 1.0.1
 * @author Jeremy Heminger <contact@jeremyheminger.com>
 * @version 1.0.x
 * @date October 2018
 **/
namespace EDI.Fabric.Libraries.Readers.Types
{
    class X12850 : IEDIextraction
    {
        private static string CTT = null;
        private static string AQC = null;
        private static string CDF = null;
        private static string MA = null;
        private static string STS = null;
        private static string STN = null;
        private static Dictionary<string, string> BEG = new Dictionary<string, string>();
        private static Dictionary<string, string> N1 = new Dictionary<string, string>();
        private static Dictionary<string, Dictionary<string, string>> POrders = new Dictionary<string, Dictionary<string, string>>();
        private static List<Loop_PO1_850> po1Loops = new List<Loop_PO1_850>();
        private static List<Loop_N1_850> N1Loops = new List<Loop_N1_850>();

        // init the interface requirements
        private static string pTheFile; 
        private static FileStream ediStream = null;
        private static string portID = "";
        public string GetPortID => portID;
        public string GetCTT => CTT;
        public string GetAQC => AQC;
        public string GetCDF => CDF;
        public string GetMA => MA;
        public string GetSTS => STS;
        public string GetSTN => STN;
        public Dictionary<string, string> GetBEG { get { return BEG; } }
        public Dictionary<string, string> GetN1 { get { return N1; } }
        public Dictionary<string, Dictionary<string, string>> GetPOrders { get { return POrders; } }

        // Special case ( Sub Classes )
        private static SubClass.Acme Acme = new SubClass.Acme();
        public static string GetAcme = null;

        /** 
         * @param string the title of the file to extract
         * @param string title of the company for whom this EDI is being received
         * @param string if not null this is a list of extra directives based on custom EDI
         *          
         * */
        public X12850(string theFile, string _portID = null, string status = "Receive")
        {
            // init
            portID = _portID;
            pTheFile = theFile;

            ediStream = getFile();

            switch (status)
            {
                case "Receive":
                    Receive();
                    break;
                default:
                    throw new Exception("The requested status does not exist");
            }
        }
        static void Receive()
        {
            using (var ediReader = new X12Reader(ediStream, LoadFactory, new X12ReaderSettings() { ContinueOnError = true }))
            {
                while (ediReader.Read())
                {
                    var items = ediReader.Item as TS850;
                    switch (portID)
                    {
                        case "Acme":
                            items = ediReader.Item as TS850acme;
                            break;
                    }
                    ExtractPO1(items);
                    ExtractCTT(items);
                    ExtractST(items);
                    ExtractBEG(items);
                    ExtractN1(items);
                }
            }
            ExtractPO1fields();
            ExtractN1fields();
        }

        // ----- START READERS


        /**
            * 
            * */
        private static void ExtractPO1(TS850 items)
        {
            if (items != null)
            {
                po1Loops.AddRange(items.PO1Loop);
            }
        }

        /**
         * 
         * */
        private static void ExtractPO1fields()
        {
            foreach (var item in po1Loops)
            {
                Dictionary<string, string> row = new Dictionary<string, string>();

                row.Add("Qty",item.PO1.QuantityOrdered_02);
                row.Add("Unit",item.PO1.UnitorBasisforMeasurementCode_03);
                row.Add("Price",item.PO1.UnitPrice_04);
                row.Add("UCode",item.PO1.BasisofUnitPriceCode_05);
                row.Add("SID",item.PO1.ProductServiceIDQualifier_06);
                row.Add("PSID",item.PO1.ProductServiceID_07);
                row.Add("PSIDQ",item.PO1.ProductServiceIDQualifier_08);
                row.Add("PSID2",item.PO1.ProductServiceID_09);
                row.Add("PSIDQ2",item.PO1.ProductServiceIDQualifier_10);
                row.Add("PSID3",item.PO1.ProductServiceID_11);

                POrders.Add(item.PO1.AssignedIdentification_01, row);

            }
        }

        /**
         * 
         * */
        private static void ExtractST(TS850 items)
        {
            if (items != null)
            {
                STN = items.SE.TransactionSetControlNumber_02;
                STS = items.SE.NumberofIncludedSegments_01;
            }
        }

        /**
         * 
         * */
        private static void ExtractN1(TS850 items)
        {
            if (items != null)
            {
                N1Loops.AddRange(items.N1Loop);
            }
        }
        /**
         * 
         * */
        private static void ExtractN1fields()
        {
            string[] prefix = new string[4];
            prefix[0] = "Shipto";
            prefix[1] = "Shipto2";
            prefix[2] = "Shipfrom";
            prefix[3] = "Shipfrom2";
            int inx = 0;
            foreach (var item in N1Loops)
            {
                if (inx < 3)
                {
                    if (null != item.N1)
                    {
                        N1.Add(prefix[inx] + "EIC", item.N1.EntityIdentifierCode_01 ?? "");
                        N1.Add(prefix[inx] + "Name", item.N1.Name_02 ?? "");
                        N1.Add(prefix[inx] + "ICQ", item.N1.IdentificationCodeQualifier_03 ?? "");
                        N1.Add(prefix[inx] + "IC", item.N1.IdentificationCode_04 ?? "");
                    }
                    else
                    {
                        N1.Add(prefix[inx] + "EIC", "");
                        N1.Add(prefix[inx] + "Name", "");
                        N1.Add(prefix[inx] + "ICQ", "");
                        N1.Add(prefix[inx] + "IC", "");
                    }
                    if (null != item.N3)
                    {
                        N1.Add(prefix[inx] + "Address", null != item.N3[0] ? item.N3[0].AddressInformation_01 : "");
                        N1.Add(prefix[inx] + "City", null != item.N4[0] ? item.N4[0].CityName_01 : "");
                        N1.Add(prefix[inx] + "State", null != item.N4[0] ? item.N4[0].StateorProvinceCode_02 : "");
                        N1.Add(prefix[inx] + "Zip", null != item.N4[0] ? item.N4[0].PostalCode_03 : "");
                        N1.Add(prefix[inx] + "Country", null != item.N4[0] ? item.N4[0].CountryCode_04 : "");
                        N1.Add(prefix[inx] + "LQ", null != item.N4[0] ? item.N4[0].LocationQualifier_05 : "");
                        N1.Add(prefix[inx] + "LI", null != item.N4[0] ? item.N4[0].LocationIdentifier_06 : "");
                    }
                    else
                    {
                        N1.Add(prefix[inx] + "Address", "");
                        N1.Add(prefix[inx] + "City", "");
                        N1.Add(prefix[inx] + "State", "");
                        N1.Add(prefix[inx] + "Zip", "");
                        N1.Add(prefix[inx] + "CC", "");
                        N1.Add(prefix[inx] + "LQ", "");
                        N1.Add(prefix[inx] + "LI", "");
                    }
                }
                inx += 1;
            }
        }

        /**
         * 
         * */
        private static void ExtractBEG(TS850 items)
        {
            if (items != null)
            {
                BEG.Add("TSPC", items.BEG.TransactionSetPurposeCode_01);
                BEG.Add("POTC", items.BEG.PurchaseOrderTypeCode_02);
                BEG.Add("PON", items.BEG.PurchaseOrderNumber_03);
                BEG.Add("RN", items.BEG.ReleaseNumber_04);
                BEG.Add("DATE", items.BEG.Date_05);
                BEG.Add("CN", items.BEG.ContractNumber_06);
                BEG.Add("AT", items.BEG.AcknowledgmentType_07);
            }
        }
        /*
         * @param TS850 * overload * 
         * * */
        private static void ExtractBEG(TS850acme items)
        {
            if (items != null)
            {
                BEG.Add("TSPC", items.BEG.TransactionSetPurposeCode_01);
                BEG.Add("POTC", items.BEG.PurchaseOrderTypeCode_02);
                BEG.Add("PON", items.BEG.PurchaseOrderNumber_03);
                BEG.Add("RN", items.BEG.ReleaseNumber_04);
                BEG.Add("DATE", items.BEG.Date_05);
                BEG.Add("CN", items.BEG.ContractNumber_06);
                BEG.Add("AT", items.BEG.AcknowledgmentType_07);
                Acme.ExtractBEG(ref items, ref GetAcme);
            }
                
        }

        /**
         * 
         * */
        private static void ExtractCTT(TS850 items)
        {
            if (items != null)
            {
                if (null != items.CTTLoop)
                {
                    if (null != items.CTTLoop.CTT)
                    {
                        CTT = items.CTTLoop.CTT.NumberofLineItems_01 ?? "0";
                    }
                    else
                    {
                        CTT = "0";
                    }
                    if (null != items.CTTLoop.AMT)
                    {
                        AQC = items.CTTLoop.AMT.AmountQualifierCode_01 ?? "";
                        CDF = items.CTTLoop.AMT.CreditDebitFlagCode_03 ?? "";
                        MA  = items.CTTLoop.AMT.MonetaryAmount_02 ?? "0";
                    }
                    else
                    {
                        AQC = "";
                        CDF = "";
                        MA  = "0";
                    }
                }
            }
        }

        // ----- END READERS


        /** 
         * 
         * */
        private static Assembly LoadFactory(MessageContext mc)
        {
            if (mc.Format.Equals("X12", StringComparison.Ordinal))
            {
                if (mc.Version.StartsWith("005010X2", StringComparison.Ordinal))
                    return Assembly.Load("EdiFabric.Templates.Hipaa");

                return Assembly.Load("EdiFabric.Templates.X12");
            }

            if (mc.Format.Equals("EDIFACT", StringComparison.Ordinal))
                return Assembly.Load("EdiFabric.Templates.Edifact");

            throw new Exception(string.Format("Transaction is not supported: Format {0} Version {1} Transaction ID {2} .", mc.Format, mc.Version, mc.Name));
        }
        /**
         * 
         * */
        private FileStream getFile()
        {
            return File.OpenRead(Program.pathToFile);
        }

        
        /**
         * DEBUG PURPOSES
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * */
        public void GetXML()
        {
            ediStream = getFile();
            List<IEdiItem> x12Items;
            using (var ediReader = new X12Reader(ediStream, LoadFactory, new X12ReaderSettings() { ContinueOnError = true }))
                x12Items = ediReader.ReadToEnd().ToList();
            var x12Transactions = x12Items.OfType<TS850>();

            System.Xml.Linq.XDocument xml = null;
            foreach (var transaction in x12Transactions)
                xml = transaction.Serialize();

            Console.Write("\n\n"+xml);
        }
    }
}
