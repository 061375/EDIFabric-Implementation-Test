namespace Edi.Fabric.Templates
{
    using System;
    using EdiFabric.Core.Annotations.Edi;
    using EdiFabric.Core.Annotations.Validation;
    using EdiFabric.Core.Model.Edi.X12;
    using EdiFabric.Templates.X12;

    /// <summary>
    /// Custom version of the purchase order template for trading partner with portid Acme
    /// </summary>
    [Serializable()]
    [Message("X12", "004010", "810")]
    public class TS810acme : TS810
    {
        [Required]
        [Pos(2)]
        public new BIGCustom1 BIG { get; set; }
    }

    [Serializable()]
    [Segment("BEG")]
    public class BIGCustom1 : BIG
    {
        //  Mark as mandatory
        [Required]
        [StringLength(1, 30)]
        [DataElement("328", typeof(X12_AN))]
        [Pos(4)]
        public new string ReleaseNumber_05 { get; set; }

        //  Add new field
        [Pos(13)]
        public string NewField_14 { get; set; }
    }
}
