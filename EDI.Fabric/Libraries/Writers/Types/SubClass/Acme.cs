using Edi.Fabric.Templates;
using EdiFabric.Templates.X12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDI.Fabric.Libraries.Writers.Types.SubClass
{
    class Acme
    {
        public void BuildTD1(Dictionary<string, string> d, ref Loop_HL_856 hlLoop1, ref TS856acme result)
        {
            //Acme
            // The ACME overide only needs 3 fields
            // For something complex this overide would utilize the sub class of ACME
            //  Repeating TD1
            hlLoop1.TD1 = new List<TD1>();
            var td11 = new TD1();
            td11.PackagingCode_01 = Constants.X12856TD101;
            td11.LadingQuantity_02 = Constants.X12856TD102; // TotalCartons --> intTotalBoxes ( need to calc @todo )
            td11.WeightQualifier_06 = Constants.X12856TD106; // IfPallets ???
            hlLoop1.TD1.Add(td11);
        }
        public void BuildREF(Dictionary<string, string> d, ref Loop_HL_856 hlLoop1, ref TS856acme result)
        {
            //  Repeating REF
            hlLoop1.REF = new List<REF>();
            var ref1 = new REF();
            ref1.ReferenceIdentificationQualifier_01 = Constants.X12856REF01;
            ref1.ReferenceIdentification_02 = d["Tracking"];
            hlLoop1.REF.Add(ref1);
        }
    }
}
