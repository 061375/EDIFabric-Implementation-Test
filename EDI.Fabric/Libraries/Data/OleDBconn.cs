using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDI.Fabric.Libraries.Data
{
    class OleDBconn
    {
        public OleDBconn(string conn)
        {
            System.Data.OleDb.OleDbConnection db = new System.Data.OleDb.OleDbConnection(conn);
        }
    }
}
