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

namespace EDI.Fabric.Libraries.Readers.Types
{
    class X12810 : IEDIextraction
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
        private static List<Loop_IT1_810> po1Loops = new List<Loop_IT1_810>();
        private static List<Loop_N1_810> N1Loops = new List<Loop_N1_810>();

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

        // Special case ( Sub Classes)
        private static SubClass.Acme Acme = new SubClass.Acme();
        public static string GetAcme = null;

        public X12810(string theFile, string _portID = null, string status = "Receive")
        {
            portID = _portID;
            pTheFile = theFile;
            ediStream = getFile(status+"/"+theFile);

            switch(status)
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
                    var items = ediReader.Item as TS810;
                    switch (portID)
                    {
                        case "Acme":
                            items = ediReader.Item as TS810acme;
                            break;
                    }
                    ExtractPO1(items);
                    ExtractN1(items);
                    ExtractBEG(items);
                    ExtractCTT(items);
                }
                ExtractPO1fields();
                ExtractN1fields();
            }
        }
        /**
         * 
         * */
        static void ExtractPO1(TS810 items)
        {
            if (items != null)
            {
                po1Loops.AddRange(items.IT1Loop);
            }
        }
        static void ExtractPO1fields()
        {
            foreach (var item in po1Loops)
            {
                Dictionary<string, string> row = new Dictionary<string, string>();
                
                row.Add("Qty", item.IT1.QuantityInvoiced_02);
                row.Add("Unit", item.IT1.UnitorBasisforMeasurementCode_03);
                row.Add("Price", item.IT1.UnitPrice_04);
                row.Add("UCode", item.IT1.BasisofUnitPriceCode_05);
                row.Add("SID", item.IT1.ProductServiceIDQualifier_06);
                row.Add("PSID", item.IT1.ProductServiceID_07);
                row.Add("PSIDQ", item.IT1.ProductServiceIDQualifier_08);
                row.Add("PSID2", item.IT1.ProductServiceID_09);
                row.Add("PSIDQ2", item.IT1.ProductServiceIDQualifier_10);
                row.Add("PSID3", item.IT1.ProductServiceID_11);

                POrders.Add(item.IT1.AssignedIdentification_01, row);
                
            }
        }
        /**
         * 
         * */
        private static void ExtractN1(TS810 items)
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
            // The 4 shipping address sections in the database
            string[] prefix = new string[4];
            prefix[0] = "Shipto";
            prefix[1] = "Shipto2";
            prefix[2] = "Shipfrom";
            prefix[3] = "Shipfrom2";

            int inx = 0;
            foreach (var item in N1Loops)
            {
                // if this exceeds 4 then it will be handled by a sub class
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
                        N1.Add(prefix[inx] + "City", item.N4.CityName_01 ?? "");
                        N1.Add(prefix[inx] + "State", item.N4.StateorProvinceCode_02 ?? "");
                        N1.Add(prefix[inx] + "Zip", item.N4.PostalCode_03 ?? "");
                        N1.Add(prefix[inx] + "Country", item.N4.CountryCode_04 ?? "");
                        N1.Add(prefix[inx] + "LQ", item.N4.LocationQualifier_05 ?? "");
                        N1.Add(prefix[inx] + "LI", item.N4.LocationIdentifier_06 ?? "");
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
        private static void ExtractBEG(TS810 items)
        {
            if (items != null)
            {
                BEG.Add("RN", items.BIG.ReleaseNumber_05);
                BEG.Add("COSN", items.BIG.ChangeOrderSequenceNumber_06);
                BEG.Add("PON", items.BIG.PurchaseOrderNumber_04);
                BEG.Add("TTC", items.BIG.TransactionTypeCode_07);
                BEG.Add("DATE", items.BIG.Date_01);
                BEG.Add("DATE2", items.BIG.Date_03);
                BEG.Add("TSPC", items.BIG.TransactionSetPurposeCode_08);
                BEG.Add("AT", items.BIG.ActionCode_09);
            }
        }
        /**
        * 
        * */
        private static void ExtractBEG(TS810acme items)
        {
            if (items != null)
            {
                BEG.Add("RN", items.BIG.ReleaseNumber_05);
                BEG.Add("COSN", items.BIG.ChangeOrderSequenceNumber_06);
                BEG.Add("PON", items.BIG.PurchaseOrderNumber_04);
                BEG.Add("TTC", items.BIG.TransactionTypeCode_07);
                BEG.Add("DATE", items.BIG.Date_01);
                BEG.Add("DATE2", items.BIG.Date_03);
                BEG.Add("TSPC", items.BIG.TransactionSetPurposeCode_08);
                BEG.Add("AT", items.BIG.ActionCode_09);
                Acme.ExtractBEG(ref items, ref GetAcme);
            }
        }
        /**
         * 
         * */
        private static void ExtractCTT(TS810 items)
        {
            if (items != null)
            {
                CTT = items.CTT.NumberofLineItems_01 ?? "0";
                MA = items.TDS.Amount_01 ?? "0";
            }
        }






        /** 
            * 
            * 
            * */
        static Assembly LoadFactory(MessageContext mc)
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
         * 
         * 
         * 
         * */
        private FileStream getFile(string theFile)
        {
            return File.OpenRead(Program.pathToFiles + theFile);
        }




        /**
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
         * 
         * */
        public void GetXML()
        {
            ediStream = getFile(pTheFile);
            List<IEdiItem> x12Items;
            using (var ediReader = new X12Reader(ediStream, LoadFactory, new X12ReaderSettings() { ContinueOnError = true }))
                x12Items = ediReader.ReadToEnd().ToList();
            var x12Transactions = x12Items.OfType<TS810>();

            System.Xml.Linq.XDocument xml = null;
            foreach (var transaction in x12Transactions)
                xml = transaction.Serialize();

            Console.WriteLine(xml);
        }
    }
}
