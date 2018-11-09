using EDI.Fabric.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDI.Fabric.Libraries.Data.Customer
{
    class General : GetASNDataInterface
    {
        static Libraries.Data.Get Getdata = new Libraries.Data.Get(EDI.Fabric.Program.currentDBconn);
        private static Dictionary<int, Dictionary<string, string>> Data = new Dictionary<int, Dictionary<string, string>>();
        // interface requirements
        public Dictionary<int, Dictionary<string, string>> GetData { get { return Data; } }

        public General(string PortID, string PONUM, string EDIType)
        {
            GetTransaction(PortID, PONUM, EDIType);
        }
        public void GetTransaction(string PortID, string PONUM, string EDIType)
        {
            Data = Getdata.GetTransaction(PortID, PONUM, EDIType);
        }
        //ASNPurpose
        public string GetASNPurpose(string PortID, string ASNID)
        {
            return Getdata.GetASNPurpose(PortID, ASNID);
        }
    }
}
