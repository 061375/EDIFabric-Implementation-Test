using EdiFabric.Templates.X12;
using System;
using System.Collections.Generic;
using System.Globalization;
using EdiFabric.Framework.Writers;
using EdiFabric.Core.Model.Edi.ErrorContexts;
using System.Diagnostics;
using EdiFabric.Core.Model.Edi.X12;
using Edi.Fabric.Templates;
/// <summary>
/// Generate and write EDI X12856 ASN document to a file
/// </summary>
namespace EDI.Fabric.Libraries.Writers.Types
{
    class X12856 
    {
        private static Dictionary<int, Dictionary<string, string>> data = new Dictionary<int, Dictionary<string, string>>();

        // Sub Classes
        private static readonly SubClass.Acme Acme = new SubClass.Acme();

        public static string GetHDASN = null;
        public static string ASNID = null;
        public static string ASNPurpose = null;
        public static string SSCC = null;
        public static string PONUM = null;
        public static string PortID = null;

        public X12856(string _PortID, string _PONUM, string _SSCC)
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
        static TS856 BuildEDI(Interfaces.GetASNDataInterface _data)
        {
            int iLength = 0;
            double dAmt = 0;
            bool firstRow = true;

            // Allow custom templates based on customer
            var result = new TS856(); // default
            // handle various customer needs
            switch (PortID)
            {
                case "ACME":
                    result = new TS856acme();
                    break;
            }
            // instantiate outside loop
            result.HLLoop = new List<Loop_HL_856>();
            var hlLoop1 = new Loop_HL_856();
            foreach (var d in data)
            {
                // this allows me to pass d to the method
                Dictionary<string, string> dd = new Dictionary<string, string>(d.Value);

                ASNID = (dd["Sono"] + SSCC).PadLeft(30, '0');
                string ASNPurpose = _data.GetASNPurpose(PortID, ASNID);

                // deal with the tature of the data returned
                if (firstRow == true)
                {
                    firstRow = false;
                    BuildST(dd, ref result);
                    BuildBSN(dd, ref result, ASNPurpose);
                } // End First Row Loop

                //  Begin Looped Content
                BuildHL(dd, ref hlLoop1, ref result);
                BuildTD1(dd, ref hlLoop1, ref result);
                //BuildTD3(dd, ref hlLoop1, ref result); // not used currently
                BuildTD5(dd, ref hlLoop1, ref result);
                BuildREF(dd, ref hlLoop1, ref result);
                //BuildDTM(dd, ref hlLoop1, ref result); // not used currently
                BuildN1(dd, ref hlLoop1, ref result);
                BuildN2(dd, ref hlLoop1, ref result);

                // Begin Incrementing Data
                iLength++;
                dAmt += (float.Parse(d.Value["ItemPrice"], CultureInfo.InvariantCulture) * int.Parse(d.Value["ItemQty"]));

            } // End data loop

            //  End HL Loop 1
            result.HLLoop.Add(hlLoop1);


            // End CT Loop

            return result;
        }
        static void BuildST(Dictionary<string, string> d, ref TS856 result)
        {
            result.ST = new ST();
            result.ST.TransactionSetIdentifierCode_01 = "856";
            // ask if this is created at runtime or pulled
            result.ST.TransactionSetControlNumber_02 = ASNID;
        }
        static void BuildBSN(Dictionary<string, string> d, ref TS856 result, string ASNPurpose)
        {

            result.BSN = new BSN();
            result.BSN.TransactionSetPurposeCode_01 = ASNPurpose; // ASNPurpose // 00 or 05
            result.BSN.ShipmentIdentification_02 = ASNID;
            result.BSN.Date_03 = DateTime.Now.ToString("yyyyMMdd");
            result.BSN.Time_04 = DateTime.Now.ToString("HHmm");
        }
        static void BuildHL(Dictionary<string, string> d, ref Loop_HL_856 hlLoop1, ref TS856 result)
        {
            //  1 is Hierarchical ID Number.
            //  S is the Hierarchical Level Code. “S” indicates "Shipment".
            //  This HL is the first HL used, and has no parent to identify.
            hlLoop1.HL = new HL();
            hlLoop1.HL.HierarchicalIDNumber_01 = Constants.X12856HL1;
            hlLoop1.HL.HierarchicalLevelCode_03 = Constants.X12856HL3;
        }
        static void BuildTD1(Dictionary<string, string> d, ref Loop_HL_856 hlLoop1, ref TS856 result)
        {
            //  Repeating TD1
            hlLoop1.TD1 = new List<TD1>(); 
            var td11 = new TD1();
            td11.PackagingCode_01 = Constants.X12856TD101;
            td11.LadingQuantity_02 = Constants.X12856TD102; // TotalCartons --> intTotalBoxes ( need to calc @todo )
            td11.WeightQualifier_06 = Constants.X12856TD106; // IfPallets ???
            td11.Weight_07 = Constants.X12856TD107; // "45582"; //GrossWeight
            td11.UnitorBasisforMeasurementCode_08 = Constants.X12856TD108;
            td11.Volume_09 = Constants.X12856TD109; //
            td11.UnitorBasisforMeasurementCode_10 = Constants.X12856TD110; //
            hlLoop1.TD1.Add(td11);
        }
        static void BuildTD1(Dictionary<string, string> d, ref Loop_HL_856 hlLoop1, ref TS856acme result)
        {
            //
            // The ACME overide only needs 3 fields
            // so this overide accesses the sub-class
            // 
            Acme.BuildTD1(d, ref hlLoop1, ref result);
        }
        static void BuildTD5(Dictionary<string, string> d, ref Loop_HL_856 hlLoop1, ref TS856 result)
        {
            //  Repeating TD5
            hlLoop1.TD5 = new List<TD5>(); 
            var td51 = new TD5();
            td51.RoutingSequenceCode_01 = Constants.X12856TD501;
            td51.IdentificationCodeQualifier_02 = Constants.X12856TD502;
            td51.IdentificationCode_03 = Constants.X12856TD503; //"JBHT"; // SCAC @todo
            td51.TransportationMethodTypeCode_04 = Constants.X12856TD504;
            hlLoop1.TD5.Add(td51);
        }
        static void BuildTD3(Dictionary<string, string> d, ref Loop_HL_856 hlLoop1, ref TS856 result)
        {
            //  Repeating TD3
            hlLoop1.TD3 = new List<TD3>(); 
            var td31 = new TD3();
            td31.EquipmentDescriptionCode_01 = Constants.X12856TD301;
            td31.EquipmentInitial_02 = "ABCD";
            td31.EquipmentNumber_03 = "07213567";
            td31.SealNumber_09 = "30394938483234";
            hlLoop1.TD3.Add(td31);
        }
        static void BuildREF(Dictionary<string, string> d, ref Loop_HL_856 hlLoop1, ref TS856 result)
        {
            //  Repeating REF
            hlLoop1.REF = new List<REF>();
            var ref1 = new REF();
            ref1.ReferenceIdentificationQualifier_01 = Constants.X12856REF01;
            ref1.ReferenceIdentification_02 = Constants.X12856REF02; // @todo -> there are several posabilities that would require sub-classing
            hlLoop1.REF.Add(ref1);
        }
        // Example
        static void BuildREF(Dictionary<string, string> d, ref Loop_HL_856 hlLoop1, ref TS856acme result)
        {
            Acme.BuildREF(d, ref hlLoop1, ref result);
        }
        static void BuildDTM(Dictionary<string, string> d, ref Loop_HL_856 hlLoop1, ref TS856 result)
        {
            var dtm1 = new DTM();
            dtm1.DateTimeQualifier_01 = Constants.X12856DTM01;
            dtm1.Date_02 = "200";
            hlLoop1.DTM.Add(dtm1);
        }
        static void BuildN1(Dictionary<string, string> d, ref Loop_HL_856 hlLoop1, ref TS856 result)
        {
            var n1Loop1 = new Loop_N1_856();
            //  ST is the Entity Identifier Code. “ST” indicates “Ship To”.
            //  WAL - MART DC 6094J - JIT is the Name (Ship To).
            //  UL is the Identification Code Qualifier. “UL” indicates
            //  “Global Location Number(GLN)”.
            //  0078742035260 is the Identification Code(GLN).
            n1Loop1.N1 = new N1();
            n1Loop1.N1.EntityIdentifierCode_01 = Constants.X12856N101; // ST "ship to"
            n1Loop1.N1.Name_02 = d["ShiptoName"];
            n1Loop1.N1.IdentificationCodeQualifier_03 = d["ShiptoIDQ"];
            n1Loop1.N1.IdentificationCode_04 = d["ShiptoID"];

            //  End N1 Loop 1
            hlLoop1.N1Loop.Add(n1Loop1);
        }
        static void BuildN2(Dictionary<string, string> d, ref Loop_HL_856 hlLoop1, ref TS856 result)
        {
            //  Begin N1 Loop 2
            var n1Loop2 = new Loop_N1_856();

            //  SF is the Entity Identifier Code (Ship From)
            //  SUPPLIER NAME is the Name. 
            n1Loop2.N1 = new N1();
            n1Loop2.N1.EntityIdentifierCode_01 = Constants.X12856N201;
            n1Loop2.N1.Name_02 = Program.ShipFromCompanyName;

            //  End N1 Loop 2
            hlLoop1.N1Loop.Add(n1Loop2);
        }
        /// <summary>
        /// Generate and write EDI document to a file
        /// </summary>
        static void WriteSingleInvoiceToFile(TS856 invoice)
        {

            //  2.  Validate it by skipping trailer validation
            MessageErrorContext errorContext;
            if (invoice.IsValid(out errorContext, true))
            {
                Debug.WriteLine("Message {0} with control number {1} is valid.", errorContext.Name,
                    errorContext.ControlNumber);

                //  3.  Write directly to a file
                // @todo this needs to pull from the database ( the location to write to )
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