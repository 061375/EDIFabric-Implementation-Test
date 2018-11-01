using EdiFabric.Templates.X12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edi.Fabric.Templates;

namespace EDI.Fabric.Libraries.Readers.Types.SubClass
{
    class Acme
    {
        /** 
         * 
         * This is a template
         * Example: A company names Acme follows TS850 for the most part but has some minor variations
         *          This sub class can be called to handle that
         *          These sub procedures coincide with their standard operations
         *          
         *          Caveat: some companies use more than one template 
         *                  - overload 
         * */
        public Acme()
        {

        }
        public void ExtractBEG(ref TS850acme items, ref string GetAcme)
        {
            if (items != null)
            {
                // If this existed in the incoming EDI in the CTT section:
                GetAcme = items.BEG.ReleaseNumber_04 ?? "0";
            }
        }
        public void ExtractBEG(ref TS810acme items, ref string GetAcme)
        {
            if (items != null)
            {
                // If this existed in the incoming EDI in the CTT section:
                GetAcme = items.BIG.ReleaseNumber_05 ?? "0";
            }
        }
    }
}
