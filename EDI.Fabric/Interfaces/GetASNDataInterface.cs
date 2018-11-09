using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDI.Fabric.Interfaces
{
    public interface GetASNDataInterface
    {
        Dictionary<int, Dictionary<string, string>> GetData { get; }

        string GetASNPurpose(string PortID, string ASNID);

        void GetTransaction(string PortID, string PONUM, string EDIType);

    }
}
