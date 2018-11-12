using EDI.Fabric.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDI.Fabric.Libraries.Data.Customer
{
    class ACMEASN : GetASNDataInterface
    {
        static Libraries.Data.Get Getdata = new Libraries.Data.Get(EDI.Fabric.Program.currentDBconn);
        private static Dictionary<int, Dictionary<string, string>> Data = new Dictionary<int, Dictionary<string, string>>();
        private static int ICN = 0;
        // interface requirements
        public Dictionary<int, Dictionary<string, string>> GetData { get { return Data; } }

        public ACMEASN(string PortID, string PONUM, string EDIType)
        {
            GetICN(PortID);
            if (0 == ICN)
            {
                // error
            } else {
                // do operations
                GetTransaction(PortID, PONUM, EDIType);
            }
        }
        //ASNPurpose
        public string GetASNPurpose(string PortID, string ASNID)
        {
            return Getdata.GetASNPurpose(PortID, ASNID);
        }
        public void GetTransaction(string PortID, string PONUM, string EDIType)
        {
            Data = Getdata.GetTransaction(PortID, PONUM, EDIType);
        }


        static void GetConfig()
        {
            //Getdata.GetConfigMain();
        }
        static Dictionary<int, Dictionary<string, string>> GetICN(string PortID)
        {
            Dictionary<int, Dictionary<string, string>> d = Getdata.GetMCN(PortID);
            // check if MCN has value
            // if true increment the value by 1
            // update this new value in the database
            // set ICN to the new value
            return d; // temp
        }

    }
}
