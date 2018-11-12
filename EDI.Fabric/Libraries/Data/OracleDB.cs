using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDI.Fabric.Interfaces;
using Oracle.ManagedDataAccess.Client;

namespace EDI.Fabric.Libraries.Data
{
    class OracleDB : DbConnection
    {
        OracleConnection con;

        public OracleDB(string conn)
        {
            Connect(conn);
        }
        void Connect(string conn)
        {
            con = new OracleConnection();
            //con.ConnectionString = "User Id=<username>;Password=<password>;Data Source=<datasource>";
            con.ConnectionString = conn;
            //con.Open();
            Console.WriteLine("Connected to Oracle" + con.ServerVersion);
        }
        void Close()
        {
            con.Close();
            con.Dispose();
        }
        public Dictionary<int, Dictionary<string, string>> Query(string sql)
        {
            if (Program.debug)
            {
                Console.WriteLine("Query:");
                Console.WriteLine(sql);
            }

            OracleCommand cmd = new OracleCommand();

            cmd.CommandText = sql;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = con;

            con.Open();
            /**
             * something to hang our results on
             * I don't expect more than a few 100 records per run
             * A simple query and loop to sort the data should suffice
             *
             * data[0] = {{key, value},{key, value}, etc..},
             * data[1] = {{key, value},{key, value}, etc..}, 
             * etc...
             * */
            Dictionary<int, Dictionary<string, string>> dReturn = new Dictionary<int, Dictionary<string, string>>();

            var reader = cmd.ExecuteReader();
            int row = 0;

            do
            {
                while (reader.Read())
                {
                    int count = reader.FieldCount;
                    Dictionary<string, string> col = new Dictionary<string, string>();
                    for (int i = 0; i < count; i++)
                    {
                        if (false == col.ContainsKey(reader.GetName(i).ToString()))
                            col.Add(reader.GetName(i).ToString(), reader.GetValue(i).ToString());
                    }
                    dReturn.Add(row, col);
                    row++;
                }
            } while (reader.NextResult());

            return dReturn;
        }
    }
}
