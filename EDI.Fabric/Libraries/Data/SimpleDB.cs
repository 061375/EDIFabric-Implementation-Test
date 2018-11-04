using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDI.Fabric
{
    /**
     * I don't expect more than a few 100 records per run
     * A simple query and loop to sort the data should suffice
     *
     * data[0] = {{key, value},{key, value}, etc..},
     * data[1] = {{key, value},{key, value}, etc..}, 
     * etc...
     * */
    class SimpleDB
    {
        private static SqlConnection sqlConnection1 = null;

        //private static SqlCommand cmd = null;

        public SimpleDB(string conn)
        {
            sqlConnection1 = new SqlConnection(conn);
        }
        public Dictionary<int, Dictionary<string, string>> Query(string sql)
        {
            
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;

            sqlConnection1.Open();

            Dictionary<int, Dictionary<string, string>> _return = new Dictionary<int, Dictionary<string, string>>();

            var reader = cmd.ExecuteReader();
            int row = 0;
            string fkey = "";
            do
            {
                
                while (reader.Read())
                {
                    int count = reader.FieldCount;
                    Dictionary<string, string> col = new Dictionary<string, string>();
                    for (int i = 0; i < count; i++)
                    {

                        // Console.WriteLine(row + " " + reader.GetName(i) + " " + reader.GetValue(i)); // debug
                        
                        if(false == col.ContainsKey(reader.GetName(i).ToString()))
                            col.Add(reader.GetName(i).ToString(), reader.GetValue(i).ToString());
                        /*
                        if(fkey == reader.GetName(i).ToString())
                        {
                            //Console.WriteLine(row + " " + reader.GetName(i) + " " + reader.GetValue(i));
                            _return.Add(row, col);
                            row++;
                            // new 
                            col.Clear();
                            col.Add(reader.GetName(i).ToString(), reader.GetValue(i).ToString());
                        }
                        else
                        {
                            // add
                            col.Add(reader.GetName(i).ToString(), reader.GetValue(i).ToString());
                        }
                        
                        if ("" == fkey)
                        {
                            fkey = reader.GetName(i);
                        }
                        */
                    }
                    _return.Add(row, col);
                    row++;
                }
            } while (reader.NextResult());

            sqlConnection1.Close();

            return _return;
        }
    }

    internal class Return
    {

    }
}
