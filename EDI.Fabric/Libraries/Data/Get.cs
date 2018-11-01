using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDI.Fabric.Libraries.Data
{
    class Get
    {
        // @SimpleDB class 
        private static SimpleDB _db;

        //private static readonly IEDIextraction _class;

        public Get(string conn)
        {
            _db = new SimpleDB(conn);
        }
        public Dictionary<int, Dictionary<string, string>> GetTransactions(string portID, string startDate, string endDate)
        {
            //01006097142
            //20181001
            //20181010

            string sql = "SELECT * FROM TransMaster " +
                        " LEFT OUTER JOIN dbo.TransDetail ON dbo.TransMaster.PONUM = dbo.TransDetail.PONUM AND dbo.TransMaster.PortID = dbo.TransDetail.PortID " +
                        " WHERE (dbo.TransMaster.PortID = '" + portID + "')" +
                        " AND (dbo.TransMaster.PODATE >= '" + startDate + "')" +
                        " AND (dbo.TransMaster.PODATE <= '" + endDate + "') " +
                        " AND (dbo.TransMaster.Status <> 0)";

            return _db.Query(sql);
        }
    }
}
