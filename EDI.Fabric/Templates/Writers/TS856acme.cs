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
    [Message("X12", "004010", "856")]
    public class TS856acme : TS856
    {
    }

    [Serializable()]
    [Segment("TD1")]
    public class TD1Custom1 : TD1
    {

    }
}
