namespace EDI.Fabric
{
    static class Constants
    {
        // --- X12856 write defaults

        // HL1
        public const string X12856HL1 = "01";
        public const string X12856HL3 = "S";
        // TD1
        public const string X12856TD101 = "CTN25";
        public const string X12856TD102 = "";
        public const string X12856TD106 = "G";
        public const string X12856TD107 = "45582";
        public const string X12856TD108 = "LB";
        public const string X12856TD109 = "";
        public const string X12856TD110 = "";
        // TD5
        public const string X12856TD501 = "";
        public const string X12856TD502 = "2";
        public const string X12856TD504 = "";
        // TD3
        public const string X12856TD301 = "TL";
        public const string X12856TD302 = "ABCD";
        public const string X12856TD303 = "07213567";
        public const string X12856TD309 = "30394938483234";
        // REF
        public const string X12856REF01 = "BM";
        public const string X12856REF02 = "01140824";
        // DTM
        public const string X12856DTM01 = "011";
        public const string X12856DTM02 = "200";
        // N1
        public const string X12856N101 = "ST";
        // N2
        public const string X12856N201 = "SF";

        // ----- X12850 write defaults

        //BEG
        public const string X12856BEG01 = "00";
        public const string X12856BEG02 = "SA";
        // N1
        public const string X12850N101 = "ST";
        // PO1
        public const string X12850PO101 = "PE";
    }
}