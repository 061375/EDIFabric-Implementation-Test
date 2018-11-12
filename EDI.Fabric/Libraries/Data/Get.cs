using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDI.Fabric.Libraries.Data
{
    class Get : IDisposable
    {
        // @SimpleDB class 
        private static SimpleDB Db;

        //private static readonly IEDIextraction _class;

        public Get(string conn)
        {
            Db = new SimpleDB(conn);
        }
        public Dictionary<int, Dictionary<string, string>> GetConfigMain()
        {
            string sql = "SELECT * FROM ConfigMain";
            return Db.Query(sql);
        }
        // override 
        public Dictionary<string, string> GetConfigMain(string PortID = null)
        {
            string sql = "SELECT * FROM ConfigMain WHERE PortID = '" + PortID + "'";
            return Db.GetFirst(sql);
        }
        public Dictionary<int, Dictionary<string, string>> GetConfig(string cType, string PortID = null)
        {
            string sql = "SELECT * FROM Config" + cType;
            if (null != PortID)
            {
                sql += " WHERE PortID = '" + PortID + "'";
            }
            return Db.Query(sql);
        }
        public Dictionary<int, Dictionary<string, string>> GetTransactions(string PortID, string startDate, string endDate)
        {
            //01006097142
            //20181001
            //20181010

            string sql = "SELECT * FROM TransMaster " +
                        " LEFT OUTER JOIN dbo.TransDetail ON dbo.TransMaster.PONUM = dbo.TransDetail.PONUM AND dbo.TransMaster.PortID = dbo.TransDetail.PortID " +
                        " WHERE (dbo.TransMaster.PortID = '" + PortID + "')" +
                        " AND (dbo.TransMaster.PODATE >= '" + startDate + "')" +
                        " AND (dbo.TransMaster.PODATE <= '" + endDate + "') " +
                        " AND (dbo.TransMaster.Status <> 0)";

            return  Db.Query(sql);
        }
        //
        public Dictionary<int, Dictionary<string, string>> GetTransaction(string PortID, string PONUM, string EDIType)
        {
            string sql = "SELECT dbo.TransMaster.*, dbo.TransDetail.ItemQty," +
                         " dbo.TransDetail.ItemUnit, dbo.TransDetail.ItemPrice, dbo.TransDetail.ItemIQ, " + 
                         " dbo.TransDetail.ItemID, dbo.TransDetail.ItemSQ, dbo.TransDetail.ItemDesc, " + 
                         " dbo.TransDetail.ItemIQ2, dbo.TransDetail.ItemID2, dbo.TransDetail.ItemIQ3, " + 
                         " dbo.TransDetail.ItemID3" +
                         " FROM dbo.TransDetail INNER JOIN" +
                         " dbo.TransMaster ON dbo.TransDetail.PortID = dbo.TransMaster.PortID AND " + 
                         " dbo.TransDetail.POID = dbo.TransMaster.POID AND " + 
                         " dbo.TransDetail.ST = dbo.TransMaster.ST AND " + 
                         " dbo.TransDetail.PONUM = dbo.TransMaster.PONUM" +
                         " WHERE (dbo.TransMaster.PONUM = '" + PONUM + "') AND (dbo.TransMaster.PortID = '" + PortID + "') AND (dbo.TransMaster.ST = '" + EDIType + "')" +
                         " ORDER BY dbo.TransDetail.ItemSQ ASC";
            return Db.Query(sql);
        }
        //
        public Dictionary<int, Dictionary<string, string>> GetSONO(string SONO)
        {
            string sql = "SELECT * FROM BOLlog WHERE (sono = '   " + SONO + "')";
            return Db.Query(sql);
        }
        //
        public Dictionary<string, string> GetMCN(string PortID)
        {
            string sql = "SELECT MCN FROM ConfigMain WHERE (PortID = '" + PortID + "')";
            return Db.GetFirst(sql);
        }
        //
        public Dictionary<string, string> GetASN(string PortID, string ASN)
        {
            string sql = "SELECT * FROM ASNlog WHERE (PortID = '" + PortID + "') AND (ASN LIKE '" + ASN + "%')";
            return Db.GetFirst(sql);
        }
        //
        public Dictionary<int, Dictionary<string, string>> GetPalletes(string SONO)
        {
            string sql = "SELECT * FROM Pallets WHERE (Sono = '   " + SONO + "') ORDER BY pQty ASC";
            return Db.Query(sql);
        }
        //
        public Dictionary<int, Dictionary<string, string>> GetPalleteContents(string PAID)
        {
            string sql = "SELECT * FROM PalletContents WHERE (PAID = " + PAID + ") ORDER BY Item ASC";
            return Db.Query(sql);
        }
        //
        public Dictionary<int, Dictionary<string, string>> GetStatus(string PONUM, string PortID, string ST)
        {
            string sql = "SELECT Status FROM TransMaster WHERE (PONUM = '" + PONUM + "') AND (PortID = '" + PortID + "') AND (ST = '" + ST + "')";
            return Db.Query(sql);
        }
        //
        public Dictionary<int, Dictionary<string, string>> GetSSCC(string PortID = null, string SSCC = null)
        {
            string sql = "SELECT * FROM SSCClog";
            if(null != PortID || null != SSCC)
            {
                sql += " WHERE ";
            }
            if (null != PortID) {
                sql += "(PortID = '" + PortID + "')";
            }
            if(null != SSCC)
            {
                if(null != PortID)
                {
                    sql += " AND ";
                }
                sql += "(SSCC = '" + SSCC + "')";
            }
            
            return Db.Query(sql);
        }
        //
        public Dictionary<int, Dictionary<string, string>> GetSSCCcontents(string SSCC)
        {
            string sql = "SELECT * FROM SSCCcontents WHERE (SSCC = '" + SSCC + "') ORDER BY ItemSQ ASC";
            return Db.Query(sql);
        }
        //
        public Dictionary<int, Dictionary<string, string>> GetCaseQty(string strCompany, string LookupKey, string ItemID)
        {
            string sql = "SELECT Caseqty, item, itemupc FROM ARINVT" + strCompany + " WHERE (" + LookupKey + " = " + (char)34 + ItemID + (char)34 + ")";
            return Db.Query(sql);
        }



        // While I generally like there to be a MODEL that ONLY accesses data
        // I think in this case it would be nice to be able to inherit some necessary operations
        // that are required after the data is pulled

        //ASNPurpose
        public string GetASNPurpose(string PortID, string ASNID)
        {
            // @todo - this needs to update the database
            // IMPORTANT !

            Dictionary<string, string> d = GetASN(PortID, ASNID);
            if (null == d["PortID"])
            {
                return "00";
            }
            else
            {
                return "05";
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
