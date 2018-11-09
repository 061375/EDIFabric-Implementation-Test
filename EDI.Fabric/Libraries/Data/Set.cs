using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDI.Fabric.Libraries.Data
{
    class Set
    {
        // @SimpleDB class 
        private static SimpleDB _db;

        private static IEDIextraction _class;

        public Set(IEDIextraction __class, string conn)
        {
            _class = __class;
            _db = new SimpleDB(conn);

            TransMaster();
            TransDetail();
        }
        static void TransDetail()
        {
            // TransDetail
            foreach (KeyValuePair<string, Dictionary<string, string>> entry in _class.GetPOrders)
            {
                string sql = "INSERT INTO TransDetail (PONUM,PODATE,ItemPrice,ItemQty) VALUES (";
                sql += "'" + entry.Key + "',";
                sql += "'" + _class.GetBEG["DATE"] + "',";
                sql += "'" + entry.Value["Price"] + "',";
                sql += "'" + entry.Value["Qty"] + "');\n";

                Console.WriteLine(sql);
            }
        }
        static void TransMaster()
        {
            // lets start with TransMaster
            string sql = "INSERT INTO TransMaster (";
            sql += "PortID,";
            sql += "ST,";
            sql += "POID,";
            sql += "Status,";
            sql += "PONUM,";
            sql += "ShipToName,";
            sql += "ShipToAddress,";
            sql += "ShipToCity,";
            sql += "ShipToState,";
            sql += "ShipToZip,";
            sql += "ShipBefore,";
            sql += "ShipAfter,";
            sql += "itemCount,";
            sql += "OrderTotal,";
            sql += "Sono,";
            sql += "PODATE,";
            sql += "ShipToCountry,";
            sql += "ShipFOB,";
            sql += "ShipToIDQ,";
            sql += "GroupCN,";
            sql += "OrderByName,";
            sql += "OrderByID,";
            sql += "OrderedByIDQ,";
            sql += "OrderedByAddress,";
            sql += "OrderedByAddress2,";
            sql += "ShipToAddress2,";
            sql += "OrderedByCity,";
            sql += "OrderedByState,";
            sql += "OrderedByState,";
            sql += "OrderedByZip,";
            sql += "OrderedByCountry,";
            sql += "CarrierName,";
            sql += "CarrierID,";
            sql += "CarrierIDQ,";
            sql += "CarrierAddress,";
            sql += "CarrierAddress2,";
            sql += "CarrierCity,";
            sql += "CarrierState,";
            sql += "CarrierZip,";
            sql += "CarrierCountry,";
            sql += "VendorName,";
            sql += "VendorID,";
            sql += "VendorIDQ,";
            sql += "VendorAddress,";
            sql += "VendorAddress2,";
            sql += "VendorCity,";
            sql += "VendorState,";
            sql += "VendorZip,";
            sql += "VendorCountry,";
            sql += "Received,";
            sql += "ShipToAddress3";
            sql += ") VALUES (";
            sql += "'" + _class.GetPortID + "',";
            sql += "'" + _class.GetSTN + "',";
            sql += "'',";
            sql += "'" + _class.GetBEG["PON"] + "',";
            sql += "'" + _class.GetN1["ShiptoName"] + "',";
            sql += "'" + _class.GetN1["ShiptoAddress"] + "',";
            sql += "'" + _class.GetN1["ShiptoCity"] + "',";
            sql += "'" + _class.GetN1["ShiptoState"] + "',";
            sql += "'" + _class.GetN1["ShiptoZip"] + "',";
            sql += "'',";
            sql += "'',";
            sql += "'',";
            sql += "'" + _class.GetMA + "',";
            sql += "'',"; // sono - sales order number
            sql += "'',"; // podate
            sql += "'" + _class.GetN1["ShiptoCountry"] + "',"; // shipto country
            sql += "'',"; // ShipFOB
            sql += "'',"; // ShipToIDQ
            sql += "'',"; // GroupCN
            sql += "'',"; // 
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'',"; //
            sql += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',"; // Received
            sql += "''"; // ShipToAddress2
            sql += ");";

            Console.WriteLine(sql + "\n");
        }
    }
}
