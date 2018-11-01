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
    [Message("X12", "004010", "850")]
    public class TS850acme : TS850
    {
        [Required]
        [Pos(2)]
        public new BEGCustom1 BEG { get; set; }
    }

    [Serializable()]
    [Segment("BEG")]
    public class BEGCustom1 : BEG
    {
        //  Mark as mandatory
        [Required]
        [StringLength(1, 30)]
        [DataElement("328", typeof(X12_AN))]
        [Pos(4)]
        public new string ReleaseNumber_04 { get; set; }

        //  Add new field
        [Pos(13)]
        public string NewField_13 { get; set; }
    }
}
