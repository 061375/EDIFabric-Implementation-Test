using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDI.Fabric
{
    public interface IEDIextraction
    {
        string GetPortID { get; }

        string GetCTT { get; }
        
        string GetAQC { get; }

        string GetCDF { get; }

        string GetMA { get; }

        string GetSTS { get; }

        string GetSTN { get; }

        Dictionary<string, string> GetBEG { get; }

        Dictionary<string, string> GetN1 { get; }

        Dictionary<string, Dictionary<string, string>> GetPOrders { get; }
  
    }
}
